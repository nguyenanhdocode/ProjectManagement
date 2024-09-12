using Application.Common.Tree;
using Application.Exceptions;
using Application.Helpers;
using Application.Models.Task;
using AutoMapper;
using AutoMapper.Configuration.Conventions;
using Core.Entities;
using DataAccess.UnifOfWork;
using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            if (!isAssignedUserJoinProject)
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

            while ((await _uow.TaskRepository.GetByIdAsync(task.Id) != null))
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

    }
}
