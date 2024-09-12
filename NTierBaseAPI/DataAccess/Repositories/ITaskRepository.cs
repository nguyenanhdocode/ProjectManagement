using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface ITaskRepository : IBaseRepository<AppTask>
    {
        /// <summary>
        /// Get task of project include NextTasks
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<AppTask> GetTaskByIdWithNextTasks(string id);
    }
}
