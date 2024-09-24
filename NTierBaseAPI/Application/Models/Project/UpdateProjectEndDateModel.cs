using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Project
{
    public class UpdateProjectEndDateModel
    {
        [FromRoute(Name = "id")]
        public string Id { get; set; }
        [FromBody]
        public UpdateProjectEndDateModelBody Body { get; set; }
    }

    public class UpdateProjectEndDateModelBody
    {
        public DateTime EndDate { get; set; }
    }
}
