using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Task
{
    public class CheckAffectBeforeUpdateEndDateModel
    {
        [FromRoute(Name = "id")]
        public string Id { get; set; }
        [FromQuery(Name = "endDate")]
        public DateTime EndDate { get; set; }
    }
}
