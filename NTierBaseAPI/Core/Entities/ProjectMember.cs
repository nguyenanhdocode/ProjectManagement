using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ProjectMember
    {
        public string ProjectId { get; set; }
        public string MemberId { get; set; }
        public DateTime JoinedDate { get; set; }
        public Project Project { get; set; }
        public AppUser Member { get; set; }
    }
}
