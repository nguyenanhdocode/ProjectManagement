using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class AppTask
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedUserId { get; set; }
        public string AssignedToUserId { get; set; }
        public string ProjectId { get; set; }
        public string ParentId { get; set; }
        public string PreviousTaskId { get; set; }
        public AppUser CreatedUser { get; set; }
        public AppUser AssignedToUser { get; set; }
        public Project Project { get; set; }
        public AppTask Parent {  get; set; }
        public AppTask PreviousTask { get; set; }
        public ICollection<AppTask> SubTasks { get; set; }
        public ICollection<AppTask> NextTasks { get; set; }
        public ICollection<TaskAsset> TaskAssets { get; set; }
    }
}
