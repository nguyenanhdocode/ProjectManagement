using Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Project
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedUserId { get; set; }
        public string ManagerId { get; set; }
        public AppUser CreatedUser { get; set; }
        public AppUser Manager {  get; set; }
        public ICollection<ProjectMember> ProjectMembers { get; set; }
        public ICollection<ProjectAsset> ProjectAssets { get; set; }
        public ICollection<AppTask> AppTasks { get; set; }
    }
}
