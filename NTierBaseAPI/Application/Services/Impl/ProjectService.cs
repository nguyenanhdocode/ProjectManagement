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
using FluentValidation.Results;
using LinqKit;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
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
        private readonly IServiceScopeFactory _scopeFactory;


        public ProjectService(IProjectRepository projectRepo
            , IMapper mapper
            , IClaimService claimService
            , IUnitOfWork unitOfWork
            , IAssetService assetService
            , ITaskService taskService
            , IServiceScopeFactory serviceScope)
        {
            _projectRepository = projectRepo;
            _mapper = mapper;
            _claimService = claimService;
            _uow = unitOfWork;
            _assetService = assetService;
            _taskService = taskService;
            _scopeFactory = serviceScope;
        }

        public async Task AddMember(AddProjectMemeberModel model)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(model.ProjectId);

            if (project == null)
                throw new NotFoundException("Project not found!");

            if (!project.CreatedUserId.Equals(_claimService.GetUserId()))
                throw new ForbiddenException();

            project.ProjectMembers = model.Body.UserIds.Select(p => new ProjectMember
            {
                ProjectId = model.ProjectId,
                MemberId = p,
                JoinedDate = DateTime.UtcNow,
            }).ToList();

            _uow.ProjectRepository.Update(project);
            await _uow.SaveChangesAsync();
        }

        public async Task AddProjectAsset(AddProjectAssetModel model)
        {
            var ids = new List<string>();

            foreach (string id in model.Body.AssetIds)
            {
                if (!(await _uow.ProjectRepository.IsAssetAdded(model.ProjectId, id)))
                    ids.Add(id);
            }

            if (ids.Count == 0)
                return;

            _uow.ProjectRepository.AddAssets(model.ProjectId, ids);
            await _uow.SaveChangesAsync();
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

        public async Task<GetAllProjectResponseModel> GetAll(ViewAllProjectModel model)
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

            if (!string.IsNullOrEmpty(model.JoinRole) && !model.JoinRole.Equals("all"))
            {
                predicate = predicate.And(p => p.ManagerId.Equals(_claimService.GetUserId()) == (model.JoinRole.Equals("manager")));
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

            var timespan = project.EndDate - project.BeginDate;
            int days = timespan.Days + 1;
            for (int i = 0; i < days; i++)
            {
                timeline.Days.Add(project.BeginDate.AddDays(i));
                timeline.Table[0].Add(null);
            }

            var predicate = PredicateBuilder.New<AppTask>(p => p.ParentId == null);

            if (!string.IsNullOrEmpty(model.Kw))
            {
                model.Kw = model.Kw.ToLower();
                predicate = predicate.And(p => p.Id.ToLower().Equals(model.Kw) || p.Name.ToLower().Contains(model.Kw));
            }

            if (!string.IsNullOrEmpty(model.Status))
            {
                var filterStatus = model.Status.Split(",").Select(p => Convert.ToInt32(p)).ToList();
                predicate = predicate.And(p => filterStatus.Any(u => u.Equals(p.Status)));
            }

            if (!string.IsNullOrEmpty(model.AssignedToUserIds)) {
                var filterUserIds = model.AssignedToUserIds.Split(",");
                predicate = predicate.And(p => filterUserIds.Any(u => u.Equals(p.AssignedToUserId)));
            }

            if (model.IsLate != null && (bool)model.IsLate)
            {
                predicate = predicate.And(p => DateTime.Compare(p.EndDate, DateTime.UtcNow) < 0 
                    && p.Status != (int)AppTaskStatus.Done);
            }
            
            var tasks = project.AppTasks.Where(predicate).ToList();

            tasks = tasks.OrderByDescending(p => p.NextTasks.Count).ToList();

            var fetchedTask = new List<string>();

            int row = 0;
            foreach (var task in tasks)
            {
                new BFS<AppTask>().TrevelTree(delegate (AppTask top)
                {
                    if (fetchedTask.Any(p => p.Equals(top.Id)))
                        return;

                    fetchedTask.Add(top.Id);

                    top.NextTasks = top.NextTasks.Where(predicate).ToList();

                    int col = 0, colSpan = 1;

                    col = (top.BeginDate - project.BeginDate).Days;
                    colSpan = (top.EndDate - top.BeginDate).Days + 1;
                    //colSpan = colSpan <= 0 ? 1 : colSpan;

                    for (int i = 0; i < colSpan; i++)
                    {
                        var cell = timeline.Table[row][col + i];
                        var currentRow = timeline.Table[row];

                        while (cell != null)
                        {
                            row += 1;

                            var newRow = new List<TimelineTask?>();

                            for (int d = 0; d < days; d++)
                                newRow.Add(null);

                            timeline.Table.Add(newRow);

                            cell = timeline.Table[row][col + i];
                            currentRow = timeline.Table[row];
                        }
                    }


                    for (int i = 0; i < colSpan; i++)
                    {
                        timeline.Table[row][col + i] = new TimelineTask
                        {
                            Id = top.Id,
                            Col = col,
                            Row = row,
                            TaskInfo = _mapper.Map<ViewTaskModel>(top),
                            Colspan = colSpan,
                            IsRendered = i == 0
                        };
                    }
                }, task, "NextTasks");
            }

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

        public async Task CheckBeforeUpdateEndDate(Models.Project.CheckBeforeUpdateEndDateModel model)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(model.Id);

            if (project == null)
                throw new NotFoundException("Project not found");

            if (project.AppTasks.Any(p => DateTime.Compare(p.EndDate, model.EndDate) > 0))
                throw new ConflictExeception("Some tasks affected");
        }

        public async Task<List<ViewTaskModel>> GetTasksOfProject(ViewProjectTasksModel model)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(model.Id);

            if (project == null)
                throw new NotFoundException("Project not found");

            var tasks = project.AppTasks.ToList();

            return _mapper.Map<List<ViewTaskModel>>(tasks);
        }

        public async Task RemoveMembers(DeleteProjectMembersModel model)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(model.ProjectId);

            if (project == null)
                throw new NotFoundException("Project not found");

            await _uow.ProjectRepository.RemoveMember(model.ProjectId, model.MemberId);

            await _uow.SaveChangesAsync();
        }

        public async Task Delete(DeleteProjectModel model)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(model.Id);

            if (project == null)
                throw new NotFoundException("Project not found");

            if (!project.ManagerId.Equals(_claimService.GetUserId()))
                throw new ForbiddenException();

            project.ProjectAssets.Clear();
            project.ProjectMembers.Clear();

            _uow.TaskRepository.DeleteRange(project.AppTasks.ToList());

            _uow.ProjectRepository.Update(project);
            _uow.ProjectRepository.Delete(project);

            await _uow.SaveChangesAsync();
        }

        public async Task<List<ViewTaskModel>> GetAvaiablePrevTasks(ViewAvaiablePrevTasksModel model)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(model.ProjectId);

            if (project == null)
                throw new NotFoundException("Project not found");

            var predicate = PredicateBuilder.New<AppTask>(p => p.ParentId == null);

            if (!string.IsNullOrEmpty(model.TaskId))
            {
                predicate = predicate.And(p => p.Id != model.TaskId && p.PreviousTaskId != model.TaskId);
            }

            var tasks = project.AppTasks.Where(predicate).ToList();

            return _mapper.Map<List<ViewTaskModel>>(tasks);
        }

        public async Task<double> GetProjectProgress(ViewProjectProgressModel model)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(model.Id);

            if (project == null)
                throw new NotFoundException("Project not found");

            var totalTasks = project.AppTasks.Where(p => p.SubTasks.Count == 0);
            var doneTasks = totalTasks.Where(p => p.Status == (int)AppTaskStatus.Done);

            return totalTasks.Count() > 0 ? (doneTasks.Count() / (double)totalTasks.Count()) * 100 : 0;
        }
    }
}
