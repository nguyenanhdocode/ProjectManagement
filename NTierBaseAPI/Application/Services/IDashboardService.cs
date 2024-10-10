using Application.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IDashboardService
    {
        /// <summary>
        /// Get today tasks
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<List<ViewTodayTasksItem>> GetTodayTasks(ViewTodayTasksModel model);

        /// <summary>
        /// Get late tasks
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<List<ViewLateTasksItem>> GetLateTasks(ViewLateTasksModel model);
    }
}
