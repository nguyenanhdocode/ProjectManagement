using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Project
{
    public class RemoveAssetOfProjectModel
    {
        [FromQuery(Name = "keep-asset")]
        public bool KeepAsset { get; set; } = true;

        [FromRoute(Name = "projectId")]
        public string ProjectId { get; set; }

        [FromRoute(Name = "assetId")]
        public Guid AssetId { get; set; }
    }
}
