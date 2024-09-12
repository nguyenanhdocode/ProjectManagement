using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Task
{
    public class PreviewBeforeUpdateEndDateModel
    {
        [FromRoute(Name = "id")]
        public string TaskId { get; set; }

        [FromBody]
        public PreviewBeforeUpdateEndDateModelBody Body { get; set; }
    }

    public class PreviewBeforeUpdateEndDateModelBody
    {
        public DateTime EndDate { get; set; }
    }

    public class PreviewBeforeUpdateEndDateResponseModel
    {
        public int TotalChangedTasks { get; set; }
        public int TotalOvertimeTasks { get; set; }
        public TimeSpan TotalOvertimeSpan { get; set; }
        public DateTime OldEndDate { get; set; }
        public DateTime NewEndDate { get; set; }
        public TaskDiffModel DetailChange { get; set; }
    }

    public class TaskDiffModel
    {
        public string TaskId { set; get; }
        public string TaskName { set; get; }
        public DateTime OldBeginDate { get; set; }
        public DateTime NewBeginDate { get; set; }
        public DateTime OldEndDate { get; set; }
        public DateTime NewEndDate { get; set; }
        public bool IsOverTime { get; set; } = false;

        public List<TaskDiffModel> Children { get;set ; } = new List<TaskDiffModel>();
    }
}
