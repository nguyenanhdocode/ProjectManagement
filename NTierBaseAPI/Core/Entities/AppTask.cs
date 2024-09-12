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
        public int Status { get; set; }
        public virtual AppUser CreatedUser { get; set; }
        public virtual AppUser AssignedToUser { get; set; }
        public virtual Project Project { get; set; }
        public virtual AppTask Parent {  get; set; }
        public virtual AppTask PreviousTask { get; set; }
        public virtual ICollection<AppTask> SubTasks { get; set; }
        public virtual ICollection<AppTask> NextTasks { get; set; }
        public virtual ICollection<TaskAsset> TaskAssets { get; set; }
    }
}
