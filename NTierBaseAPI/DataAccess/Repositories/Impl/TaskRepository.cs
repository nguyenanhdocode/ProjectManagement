using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Impl
{
    public class TaskRepository : BaseRepository<AppTask>, ITaskRepository
    {
        public async Task<AppTask> GetTaskByIdWithNextTasks(string id)
        {
            var task = await Dbcontext.AppTasks.Include(p => p.NextTasks)
                .SingleOrDefaultAsync(p => p.Id.Equals(id));

            return task;
        }
    }
}
