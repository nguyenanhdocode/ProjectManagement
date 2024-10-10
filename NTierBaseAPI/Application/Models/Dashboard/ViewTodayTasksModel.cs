using Application.Models.Task;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Dashboard
{
    public class ViewTodayTasksModel
    {
        [FromQuery(Name = "showMyTasksOnly")]
        public string? ShowMyTasksOnly {  get; set; }
    }

    public class ViewTodayTasksItem
    {
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<ViewTaskModel> Tasks { get; set; }
    }
}
