using Application.Common;
using Application.Models.Dashboard;
using Application.Models.Task;
using AutoMapper;
using Core.Entities;
using DataAccess.Repositories;
using DataAccess.UnifOfWork;
using LinqKit;

namespace Application.Services.Impl
{
    public class DashboardService : IDashboardService
    {
        private IClaimService _claimService;
        private IUnitOfWork _uow;
        private IMapper _mapper;

        public DashboardService(IClaimService claimService
            , IProjectRepository projectRepository
            , IUnitOfWork uow
            , IMapper mapper)
        {
            _claimService = claimService;
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<List<ViewLateTasksItem>> GetLateTasks(ViewLateTasksModel model)
        {
            string userId = _claimService.GetUserId();

            var predicate = PredicateBuilder.New<AppTask>(p => p.AssignedToUserId.Equals(userId));

            if (string.IsNullOrEmpty(model.ShowMyTasksOnly))
            {
                predicate = predicate.Or(p => p.Project.ManagerId.Equals(userId));
            }

            predicate = predicate.And(p => (!p.Status.Equals((int)AppTaskStatus.Done) && DateTime.Compare(DateTime.UtcNow, p.EndDate) >= 0)
                || (p.Status.Equals((int)AppTaskStatus.Unfulfilled) && DateTime.Compare(DateTime.Now, p.BeginDate) >= 0));

            var tasks = await _uow.TaskRepository.GetManyAsync(predicate);

            var results = tasks.GroupBy(p => p.ProjectId).Select((p) => new ViewLateTasksItem
            {
                ProjectId = p.Key,
                ProjectName = p.First(u => u.ProjectId.Equals(p.Key)).Project.Name,
                Tasks = _mapper.Map<List<ViewTaskModel>>(p.ToList()).OrderBy(p => $"{p.AssignedToUser.FirstName} {p.AssignedToUser.LastName}").ToList(),
            })
            .ToList();

            return results;
        }

        public async Task<List<ViewTodayTasksItem>> GetTodayTasks(ViewTodayTasksModel model)
        {
            string userId = _claimService.GetUserId();

            var predicate = PredicateBuilder.New<AppTask>(p => p.AssignedToUserId.Equals(userId));

            if (string.IsNullOrEmpty(model.ShowMyTasksOnly))
            {
                predicate = predicate.Or(p => p.Project.ManagerId.Equals(userId));
            }

            predicate = predicate.And(
               p => (p.Status.Equals((int)AppTaskStatus.Unfulfilled) && DateTime.Compare(DateTime.UtcNow.Date, p.BeginDate.Date) >= 0)
                || p.Status.Equals((int)AppTaskStatus.Doing)
            );

            var tasks = await _uow.TaskRepository.GetManyAsync(predicate);

            var results = tasks.GroupBy(p => p.ProjectId).Select((p) => new ViewTodayTasksItem
            {
                ProjectId = p.Key,
                ProjectName = p.First(u => u.ProjectId.Equals(p.Key)).Project.Name,
                Tasks = _mapper.Map<List<ViewTaskModel>>(p.ToList()).OrderBy(p => $"{p.AssignedToUser.FirstName} {p.AssignedToUser.LastName}").ToList(),
            })
            .ToList();

            return results;
        }
    }
}
