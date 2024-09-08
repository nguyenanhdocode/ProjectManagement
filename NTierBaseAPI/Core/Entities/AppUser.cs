using Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid? AvatarId { get; set; }
        public Asset Avatar { get; set; }
        public DateTime CreatedDate { get; set; }

        public ICollection<Asset> Assets { get; set; }
        public ICollection<Project> Projects { get; set; }
        public ICollection<Project> ManageProjects { get; set; }
        public ICollection<ProjectMember> ProjectMembers { get; set; }
        public ICollection<AppTask> AppTasks { get; set; }
        public ICollection<AppTask> AssignedTasks { get; set; }
    }
}
