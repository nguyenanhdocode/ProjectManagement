using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Project
{
    public class PreviewBeforeUpdateProjectEndDateModel
    {
        [FromRoute(Name = "Id")]
        public string Id { get; set; }
        [FromBody]
        public PreviewBeforeUpdateProjectEndDateModelBody Body { get; set; }
    }

    public class PreviewBeforeUpdateProjectEndDateModelBody
    {
        public DateTime EndDate { get; set; }
    }

    public class PreviewBeforeUpdateProjectEndDateResponseModel
    {
        public DateTime OldBeginDate { get; set; }
        public DateTime NewBeginDate { get; set; }
        public DateTime OldEndDate { get; set; }
        public DateTime NewEndDate { get; set; }
        public TaskDiffModel Details { get; set; }
    }
}
