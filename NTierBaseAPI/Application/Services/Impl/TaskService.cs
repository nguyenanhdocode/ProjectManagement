using Application.Common;
using Application.Exceptions;
using Application.Helpers;
using Application.Models.Task;
using AutoMapper;
using Core.Entities;
using DataAccess.UnifOfWork;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Services.Impl
{
    public class TaskService : ITaskService
    {
        private IUnitOfWork _uow;
        private IClaimService _claimService;
        private IMapper _mapper;

        public TaskService(IUnitOfWork unitOfWork
            , IClaimService claimService
            , IMapper mapper)
        {
            _uow = unitOfWork;
            _claimService = claimService;
            _mapper = mapper;
        }

        public async Task<CreateTaskResponseModel> Create(CreateTaskModel model)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(model.ProjectId);

            if (project == null)
                throw new BadRequestException("Project not found!");

            // Check if create user has joined project?
            string userId = _claimService.GetUserId();
            if (!await _uow.ProjectRepository.IsUserJoinToProject(model.ProjectId, userId)
                && !project.ManagerId.Equals(userId))
            {
                throw new ForbiddenException();
            }

            // Check if task date is between project date
            if (DateTime.Compare(model.BeginDate, project.BeginDate) < 0
                || DateTime.Compare(model.EndDate, project.EndDate) > 0)
            {
                throw new BadRequestException("Invalid BeginDate or/and EndDate");
            }

            // Check if assigned user has joined to project?
            bool isAssignedUserJoinProject = await _uow.ProjectRepository
                .IsUserJoinToProject(model.ProjectId, model.AssignedToUserId);

            if (!isAssignedUserJoinProject && !project.ManagerId.Equals(_claimService.GetUserId()))
            {
                throw new BadRequestException("Assigned user have not joined project");
            }

            // In case set previous task
            if (model.PreviousTaskId != null)
            {
                var prevTask = await _uow.TaskRepository.GetByIdAsync(model.PreviousTaskId);

                if (prevTask == null)
                    throw new BadRequestException("Previous task is not found!");

                if (!prevTask.ProjectId.Equals(project.Id))
                    throw new BadRequestException("Previous task is not in a same project");

                if (DateTime.Compare(prevTask.EndDate, model.BeginDate) > 0)
                    throw new BadRequestException("BeginDate of task must be greater than or equal to EndDate of prev task");
            }

            var task = _mapper.Map<AppTask>(model);
            string id = UniqueIdHelper.GenerateId(11);

            while ((await _uow.TaskRepository.GetByIdAsync(id)) != null)
            {
                id = UniqueIdHelper.GenerateId(11);
            }

            task.Id = id;
            task.CreatedUserId = userId;

            await _uow.TaskRepository.Add(task);
            await _uow.SaveChangesAsync();

            return new CreateTaskResponseModel
            {
                Id = task.Id
            };
        }

        public async Task<CreateSubtaskReponseModel> CreateSubTask(CreateSubtaskModel model)
        {
            var task = await _uow.TaskRepository.GetByIdAsync(model.TaskId);

            if (task == null)
                throw new NotFoundException("Task not found!");

            if (task.PreviousTaskId != null)
                throw new BadRequestException("Cannot create sub-task inside a sub-task");

            if (DateTime.Compare(task.BeginDate, model.Body.BeginDate) > 0
                || DateTime.Compare(task.EndDate, model.Body.EndDate) < 0)
            {
                throw new BadRequestException("BeginDate/EndDate of sub-task must be between BeginDate/EndDate of task");
            }

            if (!task.CreatedUserId.Equals(_claimService.GetUserId())
                        && !task.Project.ManagerId.Equals(_claimService.GetUserId()))
            {
                throw new ForbiddenException();
            }

            string id = UniqueIdHelper.GenerateId(11);

            while ((await _uow.TaskRepository.GetByIdAsync(id)) != null)
            {
                id = UniqueIdHelper.GenerateId(11);
            }

            var subTask = _mapper.Map<AppTask>(model.Body);
            subTask.Id = id;
            subTask.ProjectId = task.ProjectId;
            subTask.ParentId = task.Id;
            subTask.CreatedUserId = _claimService.GetUserId();

            await _uow.TaskRepository.Add(subTask);
            await _uow.SaveChangesAsync();

            return new CreateSubtaskReponseModel { Id = subTask.Id };
        }

        public async Task Delete(string id)
        {
            var task = await _uow.TaskRepository.GetByIdAsync(id);
            if (task == null)
                throw new NotFoundException("Task is not found!");

            if (!task.CreatedUserId.Equals(_claimService.GetUserId())
                && !task.Project.ManagerId.Equals(_claimService.GetUserId()))
            {
                throw new ForbiddenException();
            }

            _uow.TaskRepository.DeleteRange(task.SubTasks.ToList());

            task.NextTasks.Clear();
            task.TaskAssets.Clear();
            _uow.TaskRepository.Update(task);
            _uow.TaskRepository.Delete(task);

            await _uow.SaveChangesAsync();
        }

        public async Task<PreviewBeforeUpdateEndDateResponseModel> GetPreviewBeforeUpdateEndDate(
            PreviewBeforeUpdateEndDateModel model)
        {
            var task = await _uow.TaskRepository.GetTaskByIdWithNextTasks(model.TaskId);
            var project = task.Project;

            if (task == null)
                throw new BadRequestException("Task not found!");

            var timespan = model.Body.EndDate - task.EndDate;
            int totalChangeTasks = 1;
            int totalOverTimeTasks = 0;
            DateTime? latestDate = null;

            var rootTaskDiff = new TaskDiffModel()
            {
                TaskId = task.Id,
                TaskName = task.Name,
                OldBeginDate = task.BeginDate,
                OldEndDate = task.EndDate,
                NewBeginDate = task.BeginDate,
                NewEndDate = task.EndDate.Add(timespan),
                IsOverTime = DateTime.Compare(project.EndDate, task.EndDate.Add(timespan)) < 0
            };

            if (rootTaskDiff.IsOverTime)
            {
                latestDate = model.Body.EndDate;
            }

            Queue<AppTask> queue = new Queue<AppTask>();
            queue.Enqueue(task);

            Dictionary<AppTask, TaskDiffModel> nodeMap
                = new Dictionary<AppTask, TaskDiffModel>();
            nodeMap.Add(task, rootTaskDiff);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var currentCopy = nodeMap[current];

                foreach (var item in current.NextTasks)
                {
                    var copy = new TaskDiffModel()
                    {
                        TaskId = item.Id,
                        TaskName = item.Name,
                        OldBeginDate = item.BeginDate,
                        OldEndDate = item.EndDate,
                        NewBeginDate = item.BeginDate.Add(timespan),
                        NewEndDate = item.EndDate.Add(timespan),
                        IsOverTime = DateTime.Compare(project.EndDate, item.EndDate.Add(timespan)) < 0
                    };

                    currentCopy.Children.Add(copy);
                    nodeMap.Add(item, copy);
                    queue.Enqueue(item);

                    totalChangeTasks += 1;

                    if (copy.IsOverTime)
                        totalOverTimeTasks += 1;

                    if (copy.IsOverTime)
                    {
                        if (latestDate == null)
                            latestDate = copy.NewEndDate;
                        else if (DateTime.Compare(copy.NewEndDate, (DateTime)latestDate) > 0)
                            latestDate = copy.NewEndDate;
                    }
                }
            }

            var totalOvertimeSpan = latestDate != null ? (DateTime)latestDate - project.EndDate : TimeSpan.Zero;

            return new PreviewBeforeUpdateEndDateResponseModel
            {
                TotalChangedTasks = totalChangeTasks,
                TotalOvertimeTasks = totalOverTimeTasks,
                TotalOvertimeSpan = totalOvertimeSpan,
                OldEndDate = project.EndDate,
                NewEndDate = project.EndDate.Add(totalOvertimeSpan),
                DetailChange = rootTaskDiff
            };
        }

        public async Task<List<string>> Update(UpdateTaskModel model, bool updateEndDateOnly = false)
        {

            var task = await _uow.TaskRepository.GetByIdAsync(model.Id);

            if (task == null)
                throw new NotFoundException("Task not found!");

            if (!task.CreatedUserId.Equals(_claimService.GetUserId())
                && !task.Project.ManagerId.Equals(_claimService.GetUserId()))
            {
                throw new ForbiddenException();
            }

            if (DateTime.Compare(model.Body.BeginDate, task.BeginDate) != 0
                && task.PreviousTaskId != null
                && DateTime.Compare(model.Body.BeginDate, task.PreviousTask.EndDate) < 0)
            {
                throw new BadRequestException("BeginDate must be greater than or equal to EndDate of prev task");
            }

            if (DateTime.Compare(model.Body.BeginDate, task.Project.BeginDate) < 0)
            {
                throw new BadRequestException("BeginDate of task must be less than or equal to BeginDate of project");
            }

            if (DateTime.Compare(model.Body.EndDate, task.Project.EndDate) > 0)
            {
                throw new BadRequestException("EndDate of task must be less than or equal to EndDate of project");
            }

            if (!updateEndDateOnly)
            {
                if (!model.Body.AssignedToUserId.Equals(task.AssignedToUserId)
                && !await _uow.ProjectRepository.IsUserJoinToProject(task.ProjectId, model.Body.AssignedToUserId))
                {
                    throw new BadRequestException("Assigned user have not joined project");
                }

                task.Name = model.Body.Name;
                task.Note = model.Body.Note;
                task.AssignedToUserId = model.Body.AssignedToUserId;
            }

            var timespan = model.Body.EndDate - task.EndDate;
            var affectedTasks = new List<string>();

            // Update subtasks
            var subTasks = task.SubTasks;
            if (subTasks != null)
            {
                foreach (var subTask in subTasks)
                {
                    subTask.BeginDate = subTask.BeginDate.Add(timespan);
                    subTask.EndDate = subTask.EndDate.Add(timespan);

                    _uow.TaskRepository.Update(subTask);
                    await _uow.SaveChangesAsync();

                    affectedTasks.Add(subTask.Id);
                }
            }

            // Update references tasks
            if (task.NextTasks.Count > 0)
            {
                // BFS
                Queue<AppTask> queue = new Queue<AppTask>();
                List<string> visited = new List<string>();

                queue.Enqueue(task);
                visited.Add(task.Id);

                while (queue.Count > 0)
                {
                    var top = queue.Dequeue();
                    top.BeginDate = top.BeginDate.Add(timespan);
                    top.EndDate = top.EndDate.Add(timespan);

                    // Update subtasks
                    if (subTasks != null)
                    {
                        foreach (var subTask in subTasks)
                        {
                            subTask.BeginDate = subTask.BeginDate.Add(timespan);
                            subTask.EndDate = subTask.EndDate.Add(timespan);

                            _uow.TaskRepository.Update(subTask);
                            await _uow.SaveChangesAsync();

                            affectedTasks.Add(subTask.Id);
                        }
                    }

                    if (DateTime.Compare(top.EndDate, task.Project.EndDate) > 0)
                        throw new BadRequestException("EndDate of task must be less than or equal to EndDate of project");

                    _uow.TaskRepository.Update(top);
                    affectedTasks.Add(top.Id);

                    var children = top.NextTasks;
                    foreach (var item in children)
                    {
                        if (!visited.Contains(item.Id))
                        {
                            queue.Enqueue(item);
                            visited.Add(item.Id);
                        }
                    }
                }
            }

            await _uow.SaveChangesAsync();

            return affectedTasks;
        }

        public async Task UpdateStatus(UpdateTaskStatusModel model)
        {
            var task = await _uow.TaskRepository.GetByIdAsync(model.Id);

            if (task == null)
                throw new NotFoundException("Task not found!");

            if (!task.CreatedUserId.Equals(_claimService.GetUserId())
                && !task.Project.ManagerId.Equals(_claimService.GetUserId()))
            {
                throw new ForbiddenException();
            }

            var rules = new Dictionary<AppTaskStatus, List<AppTaskStatus>>();
            rules.Add(AppTaskStatus.Unfulfilled, new List<AppTaskStatus> { AppTaskStatus.Doing });
            rules.Add(AppTaskStatus.Doing, new List<AppTaskStatus> { AppTaskStatus.Done, AppTaskStatus.Suspend });
            rules.Add(AppTaskStatus.Done, new List<AppTaskStatus> { AppTaskStatus.Doing });
            rules.Add(AppTaskStatus.Suspend, new List<AppTaskStatus> { AppTaskStatus.Doing });

            var allowedNewStatus = new List<AppTaskStatus>();
            bool keyExisted = rules.TryGetValue((AppTaskStatus)task.Status, out allowedNewStatus);

            if (!keyExisted || allowedNewStatus == null 
                || !allowedNewStatus.Any(p => p.Equals(model.Body.Status)))
            {
                throw new BadRequestException("New status is not allowed on this task!");
            }

            if (task.PreviousTask != null)
            {
                if (!task.PreviousTask.Status.Equals(AppTaskStatus.Done) 
                    && model.Body.Status.Equals(AppTaskStatus.Doing))
                {
                    throw new BadRequestException("New status is not allowed on this task!");
                }
            }

            task.Status = (int)model.Body.Status;

            _uow.TaskRepository.Update(task);
            await _uow.SaveChangesAsync();
        }
    }
}

