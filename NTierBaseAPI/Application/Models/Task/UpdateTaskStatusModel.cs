using Application.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Task
{
    public class UpdateTaskStatusModel
    {
        [FromRoute(Name = "Id")]
        public string Id { get; set; }

        [FromBody]
        public UpdateTaskStatusModelBody Body { get; set; }
    }

    public class UpdateTaskStatusModelBody
    {
        public AppTaskStatus Status {  get; set; }
    }
}
