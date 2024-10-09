using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Task
{
    public class ViewTaskOverviewModel
    {
        [FromRoute(Name = "id")]
        public string Id { get; set; }
    }

    public class ViewTaskOverviewResponseModel : ViewTaskModel
    {
        public int RemainHours { get; set; }
    }
}
