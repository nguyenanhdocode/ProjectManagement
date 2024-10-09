using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Task
{
    public class ViewSubtasksModel
    {
        [FromRoute(Name = "id")]
        public string Id { get; set; }

        [FromQuery(Name = "kw")]
        public string? Kw { get; set; }
    }
}
