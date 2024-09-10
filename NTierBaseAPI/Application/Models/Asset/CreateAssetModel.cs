using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Asset
{
    public class CreateAssetModel
    {
        public IFormFile File { get; set; }
        public string? CustomId { get; set; }
    }

    public class CreateAssetResponseModel
    {
        public string Id { get; set; }
        public string AssetId { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }
    }
}
