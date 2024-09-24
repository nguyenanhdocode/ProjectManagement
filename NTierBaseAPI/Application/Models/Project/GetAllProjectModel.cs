using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Project
{
    public class GetAllProjectModel
    {
        [FromQuery(Name = "kw")]
        public string? Kw {  get; set; }

        [FromQuery(Name = "isManager")]
        public bool? isManager { get; set; } = null;

        [FromQuery(Name = "beginDate")]
        public DateTime? BeginDate { get; set; }

        [FromQuery(Name = "endDate")]
        public DateTime? EndDate { get; set; }
    }

    public class GetAllProjectResponseModel
    {
        public List<ViewProjectModel> Projects { get; set; }
    }
}
