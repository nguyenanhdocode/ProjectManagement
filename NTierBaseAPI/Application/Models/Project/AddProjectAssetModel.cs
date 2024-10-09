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
        
        [FromRoute(Name = "id")]
        public string ProjectId { get; set; }

        [FromBody]
        public AddProjectAssetModelBody Body { get; set; }
    }

    public class AddProjectAssetModelBody
    {
        public List<string> AssetIds { get; set; }
    }
}
