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
        public virtual Asset Avatar { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpires { get; set;}

        public virtual ICollection<Asset> Assets { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<Project> ManageProjects { get; set; }
        public virtual ICollection<ProjectMember> ProjectMembers { get; set; }
        public virtual ICollection<AppTask> AppTasks { get; set; }
        public virtual ICollection<AppTask> AssignedTasks { get; set; }
    }
}
