using Application.Models.Task;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Project
{
    public class ViewProjectTimeLineModel
    {
        [FromRoute(Name = "id")]
        public string Id { get; set; }

        [FromQuery(Name = "status")]
        public string? Status { get; set; }

        [FromQuery(Name = "assignedToUserIds")]
        public string? AssignedToUserIds { get; set; }

        [FromQuery(Name = "kw")]
        public string? Kw { get; set; }

        [FromQuery(Name = "isLate")]
        public bool? IsLate {  get; set; }
    }

    public class ViewProjectTimeLineResponseModel
    {
        public List<DateTime> Days { get; set; } = new List<DateTime>();
        public List<List<TimelineTask?>> Table { get; set; } = new List<List<TimelineTask?>>();
    }

    public class TimelineTask
    {
        public string Id { get; set; }
        public ViewTaskModel? TaskInfo {  get; set; }
        public double Row { get; set; }
        public double Col { get; set; }
        public double Colspan { get; set; }
        public bool IsRendered { get; set; }
        public string PreviousTaskId { get; set; }
    }
}
