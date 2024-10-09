using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Project
{
    public class ViewAllProjectModel
    {
        [FromQuery(Name = "kw")]
        public string? Kw {  get; set; }

        [FromQuery(Name = "joinRole")]
        public string? JoinRole { get; set; }

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
