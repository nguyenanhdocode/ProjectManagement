using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Project
{
    public class DeleteProjectMembersModel
    {
        [FromRoute(Name = "projectId")]
        public string ProjectId { get; set; }

        [FromRoute(Name = "memberId")]
        public string MemberId { get; set; }
    }
}
