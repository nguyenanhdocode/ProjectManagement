using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Task
{
    public class CreateSubtaskModel
    {
        [FromRoute(Name = "id")]
        public string TaskId { get; set; }

        [FromBody]
        public CreateSubtaskModelBody Body { get; set; }
    }

    public class CreateSubtaskModelBody
    {
        public string Name { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Note { get; set; }
        public string AssignedToUserId { get; set; }
    }

    public class CreateSubtaskReponseModel
    {
        public string Id { get; set; }
    }
}
