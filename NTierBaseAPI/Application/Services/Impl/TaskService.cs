using Application.Common;
using Application.Exceptions;
using Application.Helpers;
using Application.Models.Task;
using AutoMapper;
using Core.Entities;
using DataAccess.UnifOfWork;
using LinqKit;

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

        public async Task CheckAffectBeforeUpdateEndDate(CheckAffectBeforeUpdateEndDateModel model)
        {
            var task = await _uow.TaskRepository.GetByIdAsync(model.Id);

            if (task == null)
                throw new NotFoundException("Task not found");

            if (task.NextTasks.Count > 0 && DateTime.Compare(model.EndDate, task.EndDate) != 0)
                throw new ConflictExeception("Some tasks affected");
        }

        public async Task CheckCanUpdateEndDate(CheckCanUpdateTaskBeginDateModel model)
        {
            var task = await _uow.TaskRepository.GetByIdAsync(model.Id);

            if (task == null)
                throw new NotFoundException("Task not found");

            if (!task.NextTasks.Any(p => p.Status.Equals((int)AppTaskStatus.Unfulfilled)))
                throw new ConflictExeception("Can not update enddate");
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

            if (task.ParentId != null)
                throw new BadRequestException("Cannot create sub-task inside a sub-task");

            if (DateTime.Compare(task.BeginDate, model.Body.BeginDate) > 0
                || DateTime.Compare(task.EndDate, model.Body.EndDate) < 0)
            {
                throw new BadRequestException("BeginDate/EndDate of sub-task must be between BeginDate/EndDate of task");
            }

            if (!task.CreatedUserId.Equals(_claimService.GetUserId())
                && !task.AssignedToUserId.Equals(_claimService.GetUserId()))
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
                && !task.AssignedToUserId.Equals(_claimService.GetUserId())
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

        public async Task<List<AppTaskStatus>> GetAllowedNewStatus(ViewAllowedNewTaskStatusModel model)
        {
            var task = await _uow.TaskRepository.GetByIdAsync(model.Id);

            if (task == null)
                throw new NotFoundException("Task not found!");

            var rules = new Dictionary<AppTaskStatus, List<AppTaskStatus>>();
            rules.Add(AppTaskStatus.Unfulfilled, new List<AppTaskStatus> { AppTaskStatus.Doing });
            rules.Add(AppTaskStatus.Doing, new List<AppTaskStatus> { AppTaskStatus.Done, AppTaskStatus.Suspend });
            rules.Add(AppTaskStatus.Done, new List<AppTaskStatus> { AppTaskStatus.Doing, AppTaskStatus.Redo });
            rules.Add(AppTaskStatus.Suspend, new List<AppTaskStatus> { AppTaskStatus.Doing, AppTaskStatus.Redo });
            rules.Add(AppTaskStatus.Redo, new List<AppTaskStatus> { AppTaskStatus.Done, AppTaskStatus.Suspend });

            var newStatus = rules.GetValueOrDefault((AppTaskStatus)task.Status, new List<AppTaskStatus>());
            newStatus.Add((AppTaskStatus)task.Status);

            if (task.HasBeenDone)
            {
                newStatus.Remove(AppTaskStatus.Doing);
            }
            else
            {
                newStatus.Remove(AppTaskStatus.Redo);
            }

            return newStatus;
        }

        public async Task<ViewTaskOverviewResponseModel> GetOvervew(ViewTaskOverviewModel model)
        {
            var task = await _uow.TaskRepository.GetByIdAsync(model.Id);
            if (task == null)
                throw new NotFoundException("Task is not found!");

            string userId = _claimService.GetUserId();
            if (!await _uow.ProjectRepository.IsUserJoinToProject(task.ProjectId, userId)
                && !task.Project.ManagerId.Equals(userId))
            {
                throw new ForbiddenException();
            }

            return _mapper.Map<AppTask, ViewTaskOverviewResponseModel>(task);
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

        public async Task<List<ViewTaskModel>> GetSubtasks(ViewSubtasksModel model)
        {
            var task = await _uow.TaskRepository.GetByIdAsync(model.Id);

            if (task == null)
                throw new NotFoundException("Task not found!");

            var predicate = PredicateBuilder.New<AppTask>(true);

            if (!string.IsNullOrEmpty(model.Kw))
            {
                predicate.And(p => p.Id.Equals(model.Kw) || p.Name.ToLower().Contains(model.Kw.ToLower())
                    || p.Note.ToLower().Contains(model.Kw.ToLower()));
            }

            return _mapper.Map<List<ViewTaskModel>>(task.SubTasks.Where(predicate));
        }

        public async Task<ViewTaskModel> GetTask(string id)
        {
            var task = await _uow.TaskRepository.GetByIdAsync(id);

            if (task == null)
                throw new NotFoundException("Task not found!");

            return _mapper.Map<ViewTaskModel>(task);
        }

        public async Task ShiftTask(ShiftTaskModel model)
        {
            var task = await _uow.TaskRepository.GetByIdAsync(model.Id);

            if (task == null)
                throw new NotFoundException("Task not found!");

            await Update(new UpdateTaskModel
            {
                Id = model.Id,
                Body = new UpdateTaskModelBody
                {
                    BeginDate = task.BeginDate.AddMilliseconds(model.Body.Milliseconds),
                    EndDate = task.EndDate.AddMilliseconds(model.Body.Milliseconds)
                }
            }, updateDateOnly: true, skipFirst: true);
        }

        public async Task<List<string>> Update(UpdateTaskModel model, bool updateDateOnly = false, bool skipFirst = false)
        {

            var task = await _uow.TaskRepository.GetByIdAsync(model.Id);

            if (task == null)
                throw new NotFoundException("Task not found!");

            if (!task.CreatedUserId.Equals(_claimService.GetUserId())
                && !task.Project.ManagerId.Equals(_claimService.GetUserId())
                && !task.AssignedToUserId.Equals(_claimService.GetUserId()))
            {
                throw new ForbiddenException();
            }

            if (DateTime.Compare(model.Body.BeginDate, task.BeginDate) != 0
                && task.PreviousTaskId != null
                && DateTime.Compare(model.Body.BeginDate, task.PreviousTask.EndDate) < 0
                && task.DoneDate != null
                && DateTime.Compare(model.Body.BeginDate, (DateTime)task.PreviousTask.DoneDate) < 0)
            {
                throw new BadRequestException("BeginDate must be greater than or equal to EndDate of prev task");
            }

            if (DateTime.Compare(model.Body.EndDate, task.Project.EndDate) > 0)
            {
                throw new BadRequestException("EndDate of task must be less than or equal to EndDate of project");
            }

            if (!updateDateOnly)
            {
                if (!model.Body.AssignedToUserId.Equals(task.AssignedToUserId)
                && !await _uow.ProjectRepository.IsUserJoinToProject(task.ProjectId, model.Body.AssignedToUserId))
                {
                    throw new BadRequestException("Assigned user have not joined project");
                }

                task.Name = model.Body.Name;
                task.Note = model.Body.Note;
                task.AssignedToUserId = model.Body.AssignedToUserId;
                task.Status = model.Body.Status;

                if (model.Body.Status == (int)AppTaskStatus.Done)
                {
                    task.HasBeenDone = true;
                    task.DoneDate = DateTime.UtcNow;
                }
            }

            var timespan = model.Body.EndDate - task.EndDate;
            var affectedTasks = new List<string>();

            // Update references tasks
            bool isFirst = true;
            new BFS<AppTask>().TrevelTree(delegate (AppTask top)
            {
                if (skipFirst && isFirst)
                {
                    isFirst = false;
                    return;
                }

                if (isFirst)
                {
                    top.BeginDate = model.Body.BeginDate;
                    top.EndDate = model.Body.EndDate;
                    isFirst = false;
                }
                else
                {
                    top.BeginDate = top.BeginDate.Add(timespan);
                    top.EndDate = top.EndDate.Add(timespan);
                }

                if (DateTime.Compare(top.BeginDate, task.Project.BeginDate) < 0)
                {
                    throw new BadRequestException("BeginDate of task must be less than or equal to BeginDate of project");
                }

                // Update subtasks
                if (top.SubTasks.Count > 0)
                {
                    foreach (var subTask in top.SubTasks)
                    {
                        subTask.BeginDate = subTask.BeginDate.Add(timespan);
                        subTask.EndDate = subTask.EndDate.Add(timespan);

                        affectedTasks.Add(subTask.Id);
                    }
                }

            }, task, "NextTasks");

            _uow.TaskRepository.Update(task);

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
            rules.Add(AppTaskStatus.Doing, new List<AppTaskStatus> { AppTaskStatus.Suspend, AppTaskStatus.Done });
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

        public async Task UpdateSubtask(UpdateSubtaskModel model)
        {
            var task = await _uow.TaskRepository.GetByIdAsync(model.Id);

            if (task == null)
                throw new NotFoundException("Task not found!");

            if (!task.CreatedUserId.Equals(_claimService.GetUserId())
                && !task.Project.ManagerId.Equals(_claimService.GetUserId())
                && !task.AssignedToUserId.Equals(_claimService.GetUserId()))
            {
                throw new ForbiddenException();
            }

            if (model.Body.Status == (int)AppTaskStatus.Doing && task.Status == (int)AppTaskStatus.Unfulfilled)
                task.Parent.Status = (int)AppTaskStatus.Doing;

            task.Name = model.Body.Name;
            task.BeginDate = model.Body.BeginDate;
            task.EndDate = model.Body.EndDate;
            task.Status = model.Body.Status;
            task.Note = model.Body.Note;

            _uow.TaskRepository.Update(task);
            await _uow.SaveChangesAsync();
        }
    }
}

