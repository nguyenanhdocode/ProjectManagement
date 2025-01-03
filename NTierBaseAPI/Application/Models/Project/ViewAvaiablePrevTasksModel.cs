﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Project
{
    public class ViewAvaiablePrevTasksModel
    {
        [FromRoute(Name = "id")]
        public string ProjectId { get; set; }

        [FromQuery(Name = "taskId")]
        public string? TaskId { get; set; }
    }
}
