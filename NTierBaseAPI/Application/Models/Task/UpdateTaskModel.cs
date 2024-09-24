using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Task
{
    public class UpdateTaskModel
    {
        [FromRoute(Name = "id")]
        public string Id { get; set; }

        [FromBody]
        public UpdateTaskModelBody Body { get; set; }
    }

    public class UpdateTaskModelBody
    {
        public string Name { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Note { get; set; }
        public string AssignedToUserId { get; set; }
    }
}
