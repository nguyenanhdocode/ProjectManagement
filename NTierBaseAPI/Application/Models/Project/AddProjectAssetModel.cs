using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Project
{
    public class AddProjectAssetModel
    {
        public IFormFile File { get; set; }
        [FromRoute(Name = "projectId")]
        public string ProjectId { get; set; }
    }
}
