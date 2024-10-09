﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Task
{
    public class ShiftTaskModel
    {
        [FromRoute(Name = "id")]
        public string Id { get; set; }

        [FromBody]
        public ShiftTaskModelBody Body { get; set; }
    }

    public class ShiftTaskModelBody
    {
        public double Milliseconds { get; set; }
    }
}