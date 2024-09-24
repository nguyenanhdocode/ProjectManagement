using Application.Models.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Project
{
    public class ViewProjectTimeLineModel
    {
        public string Id { get; set; }
    }

    public class ViewProjectTimeLineResponseModel
    {
        public List<DateTime> Days { get; set; } = new List<DateTime>();
        public List<List<TimelineTask?>> Table { get; set; } = new List<List<TimelineTask?>>();
    }

    public class TimelineTask
    {
        public ViewTaskModel? TaskInfo {  get; set; }
        public double Row { get; set; }
        public double Col { get; set; }
        public double Colspan { get; set; }
        public bool IsRendered { get; set; }
    }
}
