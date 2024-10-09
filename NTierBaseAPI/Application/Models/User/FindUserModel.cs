using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.User
{
    public class FindUserModel
    {
        [FromQuery(Name = "kw")]
        public string? Kw { get; set; }
    }
}
