using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Project
{
    public class UpdateProjectModel
    {
        [FromRoute(Name = "id")]
        public string Id { get; set; }
        [FromBody]
        public UpdateProjectModelBody Body { get; set; }
    }

    public class UpdateProjectModelBody
    {
        public string Name { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public string ManagerId { get; set; }
    }
}
