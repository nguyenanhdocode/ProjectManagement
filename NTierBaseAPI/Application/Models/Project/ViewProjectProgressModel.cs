﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Project
{
    public class ViewProjectProgressModel
    {
        [FromRoute(Name = "id")]
        public string Id { get; set; }
    }
}
