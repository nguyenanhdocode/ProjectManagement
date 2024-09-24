using Application.Common;
using Application.Exceptions;
using Application.Helpers;
using Application.Models.Asset;
using Application.Models.Project;
using Application.Models.Task;
using Application.Models.User;
using Application.Validators.Project;
using AutoMapper;
using Core.Entities;
using DataAccess.Repositories;
using DataAccess.UnifOfWork;
using LinqKit;
using System.Transactions;

namespace Application.Services.Impl
{
    public class ProjectService : IProjectService
    {
        private IProjectRepository _projectRepository;
        private IMapper _mapper;
        private IClaimService _claimService;
        private IUnitOfWork _uow;
        private IAssetService _assetService;
        private ITaskService _taskService;

        public ProjectService(IProjectRepository projectRepo
            , IMapper mapper
            , IClaimService claimService
            , IUnitOfWork unitOfWork
            , IAssetService assetService
            , ITaskService taskService)
        {
            _projectRepository = projectRepo;
            _mapper = mapper;
            _claimService = claimService;
            _uow = unitOfWork;
            _assetService = assetService;
            _taskService = taskService;
        }

        public async Task AddMember(AddProjectMemeberModel model)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(model.ProjectId);

            if (project == null)
                throw new NotFoundException("Project not found!");

            if (!project.CreatedUserId.Equals(_claimService.GetUserId()))
                throw new ForbiddenException();

            project.ProjectMembers = new List<ProjectMember>
            {
                new ProjectMember
                {
                    ProjectId = model.ProjectId,
                    MemberId = model.Body.UserId,
                    JoinedDate = DateTime.UtcNow,
                }
            };

            _uow.ProjectRepository.Update(project);
            await _uow.SaveChangesAsync();
        }

        public async Task AddProjectAsset(AddProjectAssetModel model)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeOption.Required
                , new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }
                , TransactionScopeAsyncFlowOption.Enabled))
                {
                    var project = await _uow.ProjectRepository.GetByIdAsync(model.ProjectId);

                    if (project == null)
                        throw new BadRequestException("Project not found!");

                    var uploadResult = await _assetService.Upload(new Models.Asset.CreateAssetModel
                    {
                        File = model.File,
                    });

                    project.ProjectAssets = new List<ProjectAsset>()
                    {
                        new ProjectAsset
                        {
                            ProjectId = model.ProjectId,
                            AssetId = Guid.Parse(uploadResult.Id)
                        }
                    };

                    _uow.ProjectRepository.Update(project);
                    await _uow.SaveChangesAsync();

                    tran.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CreateProjectResponseModel> Create(CreateProjectModel model)
        {
            string id = UniqueIdHelper.GenerateId(11);
            while (await _projectRepository.GetByIdAsync(id) != null)
            {
                id = UniqueIdHelper.GenerateId(11);
            }

            var project = _mapper.Map<Project>(model);
            project.Id = id;
            project.CreatedUserId = _claimService.GetUserId();
            project.ManagerId = _claimService.GetUserId();

            await _uow.ProjectRepository.Add(project);
            await _uow.SaveChangesAsync();

            return new CreateProjectResponseModel { Id = id };
        }

        public async Task<GetAllProjectResponseModel> GetAll(GetAllProjectModel model)
        {
            string userId = _claimService.GetUserId();
            var predicate = PredicateBuilder.New<Project>(p => p.ManagerId.Equals(userId)
            || p.ProjectMembers.SingleOrDefault(u => u.ProjectId.Equals(p.Id) && u.MemberId.Equals(userId)) != null);

            if (!string.IsNullOrEmpty(model.Kw))
            {
                predicate = predicate.And(p => p.Name.ToLower().Contains(model.Kw.ToLower())
                    || p.Description.ToLower().Contains(model.Kw.ToLower())
                    || p.Id.Equals(model.Kw));
            }

            if (model.BeginDate != null && model.EndDate == null)
            {
                predicate = predicate.And(p => DateTime.Compare(p.BeginDate, (DateTime)model.BeginDate) >= 0);
            }

            if (model.EndDate != null && model.BeginDate == null)
            {
                predicate = predicate.And(p => DateTime.Compare(p.EndDate, (DateTime)model.EndDate) <= 0);
            }

            if (model.BeginDate != null && model.EndDate != null)
            {
                predicate = predicate.And(p => DateTime.Compare(p.BeginDate, (DateTime)model.BeginDate) >= 0
                && DateTime.Compare(p.EndDate, (DateTime)model.EndDate) <= 0);
            }

            if (model.isManager != null)
            {
                predicate = predicate.And(p => p.ManagerId.Equals(_claimService.GetUserId()) == model.isManager);
            }

            var projects = await _uow.ProjectRepository.GetManyAsync(predicate);

            return new GetAllProjectResponseModel
            {
                Projects = _mapper.Map<List<ViewProjectModel>>(projects)
            };
        }

        public async Task<List<ViewAssetModel>> GetAssetsOfProject(string projectId)
        {
            var assets = await _projectRepository.GetAssetsOfProject(projectId);
            return _mapper.Map<List<ViewAssetModel>>(assets);
        }

        public async Task<PreviewBeforeUpdateProjectBeginDateResponseModel> GetPreviewBeforeUpdateBeginDate(
            PreviewBeforeUpdateProjectBeginDateModel model)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(model.Id);

            if (project == null)
                throw new NotFoundException("Project not found");

            if (!project.ManagerId.Equals(_claimService.GetUserId()))
                throw new ForbiddenException();

            var affectedTasks = project.AppTasks
                .Where(p => DateTime.Compare(model.Body.BeginDate, p.BeginDate) > 0
                    && p.PreviousTask == null)
                .ToList();

            var rootTaskDiff = new Models.Project.TaskDiffModel();
            var preview = new PreviewBeforeUpdateProjectBeginDateResponseModel();

            if (affectedTasks.Count > 0)
            {
                var earliestTimeSpan = model.Body.BeginDate - affectedTasks.Min(p => p.BeginDate);
                preview.OldBeginDate = project.BeginDate;
                preview.NewBeginDate = model.Body.BeginDate;
                preview.OldEndDate = project.EndDate;
                preview.NewEndDate = project.EndDate.Add(earliestTimeSpan);
                preview.Details = rootTaskDiff;

                foreach (var affectedTask in affectedTasks)
                {
                    var timespan = model.Body.BeginDate - affectedTask.BeginDate;

                    Queue<AppTask> queue = new Queue<AppTask>();
                    queue.Enqueue(affectedTask);

                    rootTaskDiff.TaskId = affectedTask.Id;
                    rootTaskDiff.TaskName = affectedTask.Name;
                    rootTaskDiff.OldBeginDate = affectedTask.BeginDate;
                    rootTaskDiff.OldEndDate = affectedTask.EndDate;
                    rootTaskDiff.NewBeginDate = affectedTask.BeginDate.Add(timespan);
                    rootTaskDiff.NewEndDate = affectedTask.EndDate.Add(timespan);

                    Dictionary<AppTask, Models.Project.TaskDiffModel> nodeMap
                        = new Dictionary<AppTask, Models.Project.TaskDiffModel>();
                    nodeMap.Add(affectedTask, rootTaskDiff);

                    while (queue.Count > 0)
                    {
                        var current = queue.Dequeue();
                        var currentCopy = nodeMap[current];

                        foreach (var item in current.NextTasks)
                        {
                            var copy = new Models.Project.TaskDiffModel()
                            {
                                TaskId = item.Id,
                                TaskName = item.Name,
                                OldBeginDate = item.BeginDate,
                                OldEndDate = item.EndDate,
                                NewBeginDate = item.BeginDate.Add(timespan),
                                NewEndDate = item.EndDate.Add(timespan),
                            };

                            currentCopy.Children.Add(copy);
                            nodeMap.Add(item, copy);
                            queue.Enqueue(item);
                        }
                    }
                }
            }

            return preview;
        }

        public async Task<PreviewBeforeUpdateProjectEndDateResponseModel> GetPreviewBeforeUpdateEndDate(
            PreviewBeforeUpdateProjectEndDateModel model)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(model.Id);

            if (project == null)
                throw new NotFoundException("Project not found");

            if (!project.ManagerId.Equals(_claimService.GetUserId()))
                throw new ForbiddenException();

            var affectedTasks = project.AppTasks
                .Where(p => DateTime.Compare(p.EndDate, model.Body.EndDate) > 0
                    && p.NextTasks.Count() == 0)
                .ToList();

            var rootTaskDiff = new Models.Project.TaskDiffModel();
            var preview = new PreviewBeforeUpdateProjectEndDateResponseModel();

            if (affectedTasks.Count > 0)
            {
                var maxOvertimeSpan = affectedTasks.Max(p => p.EndDate) - model.Body.EndDate;
                var fetchedTaskIds = new List<string>();

                preview.OldBeginDate = project.BeginDate;
                preview.NewBeginDate = project.BeginDate.Subtract(maxOvertimeSpan);
                preview.OldEndDate = project.EndDate;
                preview.NewEndDate = model.Body.EndDate;
                preview.Details = rootTaskDiff;

                foreach (var affectedTask in affectedTasks)
                {
                    var timespan = affectedTask.EndDate - model.Body.EndDate;
                    var root = await _uow.TaskRepository.GetRootTask(affectedTask.Id);

                    if (root == null || fetchedTaskIds.Any(p => p.Equals(root.Id)))
                        continue;

                    // BFS
                    Queue<AppTask> queue = new Queue<AppTask>();
                    queue.Enqueue(root);

                    rootTaskDiff.TaskId = root.Id;
                    rootTaskDiff.TaskName = root.Name;
                    rootTaskDiff.OldBeginDate = root.BeginDate;
                    rootTaskDiff.OldEndDate = root.EndDate;
                    rootTaskDiff.NewBeginDate = root.BeginDate.Subtract(timespan);
                    rootTaskDiff.NewEndDate = root.EndDate.Subtract(timespan);

                    Dictionary<AppTask, Models.Project.TaskDiffModel> nodeMap
                        = new Dictionary<AppTask, Models.Project.TaskDiffModel>();
                    nodeMap.Add(root, rootTaskDiff);

                    while (queue.Count > 0)
                    {
                        var current = queue.Dequeue();
                        var currentCopy = nodeMap[current];

                        foreach (var item in current.NextTasks)
                        {
                            var copy = new Models.Project.TaskDiffModel()
                            {
                                TaskId = item.Id,
                                TaskName = item.Name,
                                OldBeginDate = item.BeginDate,
                                OldEndDate = item.EndDate,
                                NewBeginDate = item.BeginDate.Subtract(timespan),
                                NewEndDate = item.EndDate.Subtract(timespan),
                            };

                            currentCopy.Children.Add(copy);
                            nodeMap.Add(item, copy);
                            queue.Enqueue(item);
                        }
                    }

                    fetchedTaskIds.Add(root.Id);
                }
            }

            return preview;
        }

        public async Task IsUserJoinToProject(IsUserJoinToProjectModel model)
        {
            bool isJoined = await _projectRepository.IsUserJoinToProject(model.ProjectId, model.UserId);
            if (!isJoined)
                throw new NotFoundException("User have not joined project");
        }

        public async Task RemoveAssetOfProject(RemoveAssetOfProjectModel model)
        {
            await _projectRepository.RemoveAssetOfProject(model.ProjectId, model.AssetId);

            if (!model.KeepAsset)
            {
                await _assetService.DeleteAsset(model.AssetId);
            }

            await _uow.SaveChangesAsync();
        }

        public async Task Update(UpdateProjectModel model)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(model.Id);

            if (project == null)
                throw new NotFoundException("Project not found");

            if (!project.ManagerId.Equals(_claimService.GetUserId()))
                throw new ForbiddenException();

            project.Name = model.Body.Name;
            project.Description = model.Body.Description;
            project.ManagerId = model.Body.ManagerId;

            _uow.ProjectRepository.Update(project);
            await _uow.SaveChangesAsync();
        }

        public async Task UpdateBeginDate(UpdateProjectBeginDateModel model)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeOption.Required
                    , new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }
                    , TransactionScopeAsyncFlowOption.Enabled))
                {
                    var project = await _uow.ProjectRepository.GetByIdAsync(model.Id);

                    if (project == null)
                        throw new NotFoundException("Project not found");

                    if (!project.ManagerId.Equals(_claimService.GetUserId()))
                        throw new ForbiddenException();

                    var affectedTasks = project.AppTasks
                        .Where(p => DateTime.Compare(model.Body.BeginDate, p.BeginDate) > 0
                            && p.PreviousTask == null)
                        .ToList();

                    if (affectedTasks.Count > 0)
                    {
                        var earliestTimeSpan = model.Body.BeginDate - affectedTasks.Min(p => p.BeginDate);

                        project.BeginDate = model.Body.BeginDate;
                        project.EndDate = project.EndDate.Add(earliestTimeSpan);

                        _uow.ProjectRepository.Update(project);
                        await _uow.SaveChangesAsync();

                        foreach (var task in affectedTasks)
                        {
                            var timespan = model.Body.BeginDate - task.BeginDate;
                            await _taskService.Update(new UpdateTaskModel
                            {
                                Id = task.Id,
                                Body = new UpdateTaskModelBody
                                {
                                    BeginDate = task.BeginDate.Add(timespan),
                                    EndDate = task.EndDate.Add(timespan)
                                }
                            }, true);
                        }
                    }
                    else
                    {
                        project.BeginDate = model.Body.BeginDate;
                        await _uow.SaveChangesAsync();
                    }

                    tran.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateEndDate(UpdateProjectEndDateModel model)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeOption.Required
                    , new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }
                    , TransactionScopeAsyncFlowOption.Enabled))
                {
                    var project = await _uow.ProjectRepository.GetByIdAsync(model.Id);

                    if (project == null)
                        throw new NotFoundException("Project not found");

                    if (!project.ManagerId.Equals(_claimService.GetUserId()))
                        throw new ForbiddenException();

                    var affectedTasks = project.AppTasks
                        .Where(p => DateTime.Compare(p.EndDate, model.Body.EndDate) > 0
                            && p.NextTasks.Count == 0)
                        .ToList();

                    if (affectedTasks.Count > 0)
                    {
                        var maxOverTimeSpan = affectedTasks.Max(p => p.EndDate) - model.Body.EndDate;

                        project.BeginDate = project.BeginDate.Subtract(maxOverTimeSpan);
                        project.EndDate = model.Body.EndDate;

                        _uow.ProjectRepository.Update(project);
                        await _uow.SaveChangesAsync();

                        var updatedTaskIds = new List<string>();

                        foreach (var task in affectedTasks)
                        {
                            if (updatedTaskIds.Any(p => p.Equals(task.Id)))
                                continue;

                            var timespan = task.EndDate - model.Body.EndDate;
                            var root = task;
                            while (root.PreviousTask != null)
                                root = root.PreviousTask;


                            var affectedIds = await _taskService.Update(new UpdateTaskModel
                            {
                                Id = root.Id,
                                Body = new UpdateTaskModelBody
                                {
                                    BeginDate = root.BeginDate.Subtract(timespan),
                                    EndDate = root.EndDate.Subtract(timespan)
                                }
                            }, true);

                            updatedTaskIds.AddRange(affectedIds);
                        }
                    }
                    else
                    {
                        project.EndDate = model.Body.EndDate;

                        _uow.ProjectRepository.Update(project);
                        await _uow.SaveChangesAsync();
                    }

                    tran.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ViewProjectOverviewModel> GetProjectOverview(string id)
        {
            var detail = new ViewProjectOverviewModel();

            var project = await _uow.ProjectRepository.GetByIdAsync(id);
            detail = _mapper.Map<ViewProjectOverviewModel>(project);

            return detail;
        }

        public async Task<ViewProjectTimeLineResponseModel> GetProjectTimeline(ViewProjectTimeLineModel model)
        {
            var timeline = new ViewProjectTimeLineResponseModel();
            timeline.Table.Add(new List<TimelineTask?>());

            var project = await _uow.ProjectRepository.GetByIdAsync(model.Id);

            if (project == null)
                throw new NotFoundException("Project not found");

            int days = (int)Math.Round((project.EndDate - project.BeginDate).TotalDays + 1);
            for (int i = 0; i < days; i++)
            {
                timeline.Days.Add(project.BeginDate.AddDays(i));
                timeline.Table[0].Add(null);
            }

            var tasks = project.AppTasks.Where(p => (p.PreviousTaskId == null && p.NextTasks.Count == 0)
            || (p.PreviousTaskId == null && p.NextTasks.Count > 0))
                .ToList();

            tasks = tasks.OrderByDescending(p => p.NextTasks.Count).ToList();

            foreach (var task in tasks)
            {
                new BFS<AppTask>(delegate (ref AppTask top)
                {
                    int row = 0, col = 0, colSpan = 1;

                    col = (int)Math.Round((top.BeginDate - project.BeginDate).TotalDays);
                    colSpan = (int)(top.EndDate - top.BeginDate).TotalDays;

                    //for (int i = 0; i < colSpan; i++)
                    //{
                        while (timeline.Table[row][col] != null)
                        {
                            row += 1;

                            var newRow = new List<TimelineTask?>();

                            for (int d = 0; d < days; d++)
                                newRow.Add(null);

                            timeline.Table.Add(newRow);
                        }
                    //}



                    //for (int i = 0; i < colSpan; i++)
                    //{
                    //    timeline.Table[row][col + i] = new TimelineTask
                    //    {
                    //        Col = col,
                    //        Row = row,
                    //        TaskInfo = _mapper.Map<ViewTaskModel>(top),
                    //        Colspan = colSpan,
                    //        IsRendered = i == 0
                    //    };
                    //}
                }, "NextTasks").TrevelTree(task);
            }

            timeline.Table = timeline.Table.Where(p => p.Any(m => m != null)).ToList();

            return timeline;
        }

        public async Task<List<ViewProfileModel>> GetProjectMembers(string id)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(id);

            if (project == null)
                throw new NotFoundException("Project not found");

            var members = await _uow.ProjectRepository.GetProjectMembers(id);
            members.Insert(0, project.Manager);

            return _mapper.Map<List<ViewProfileModel>>(members);
        }

        public async Task<ViewProjectModel> GetProject(string id)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(id);

            if (project == null)
                throw new NotFoundException("Project not found");

            return _mapper.Map<ViewProjectModel>(project);
        }

        public async Task CheckBeforeUpdateBeginDate(CheckBeforeUpdateBeginDateModel model)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(model.Id);

            if (project == null)
                throw new NotFoundException("Project not found");

            if (project.AppTasks.Any(p => DateTime.Compare(model.BeginDate, p.BeginDate) > 0))
                throw new ConflictExeception("Some tasks affected");
        }
    }
}
