using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Task
{
    public class CheckCanUpdateTaskBeginDateModel
    {
        [FromRoute(Name = "Id")]
        public string Id { get; set; }
    }
}
