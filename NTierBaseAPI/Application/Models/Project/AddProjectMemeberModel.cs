using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Project
{
    public class AddProjectMemeberModel
    {
        [FromRoute(Name = "id")]
        public string ProjectId { get; set; }

        [FromBody]
        public AddProjectMemeberModelBody Body { get; set; }
    }

    public class AddProjectMemeberModelBody
    {
        public List<string> UserIds { get; set; }
    }
}
