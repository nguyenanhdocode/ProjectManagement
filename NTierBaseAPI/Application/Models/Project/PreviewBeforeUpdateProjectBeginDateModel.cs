using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Project
{
    public class PreviewBeforeUpdateProjectBeginDateModel
    {
        [FromRoute(Name = "Id")]
        public string Id { get; set; }
        [FromBody]
        public PreviewBeforeUpdateProjectBeginDateModelBody Body { get; set; }
    }

    public class PreviewBeforeUpdateProjectBeginDateModelBody
    {
        public DateTime BeginDate { get; set; }
    }

    public class PreviewBeforeUpdateProjectBeginDateResponseModel
    {
        public DateTime OldBeginDate { get; set; }
        public DateTime NewBeginDate { get; set; }
        public DateTime OldEndDate { get; set; }
        public DateTime NewEndDate { get; set; }
        public TaskDiffModel Details { get; set; }
    }

    public class TaskDiffModel
    {
        public string TaskId { set; get; }
        public string TaskName { set; get; }
        public DateTime OldBeginDate { get; set; }
        public DateTime NewBeginDate { get; set; }
        public DateTime OldEndDate { get; set; }
        public DateTime NewEndDate { get; set; }
        public List<TaskDiffModel> Children { get; set; } = new List<TaskDiffModel>();
    }
}
