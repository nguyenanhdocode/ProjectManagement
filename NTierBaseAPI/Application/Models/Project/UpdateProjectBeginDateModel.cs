using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Project
{
    public class UpdateProjectBeginDateModel
    {
        [FromRoute(Name = "id")]
        public string Id { get; set; }
        [FromBody]
        public UpdateProjectBeginDateModelBody Body { get; set; }
    }

    public class UpdateProjectBeginDateModelBody
    {
        public DateTime BeginDate { get; set; }
    }
}
