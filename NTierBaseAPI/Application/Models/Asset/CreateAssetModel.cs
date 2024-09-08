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
    public class UploadImageModel
    {
        public IFormFile File { get; set; }
    }

    public class UploadImageResponseModel
    {
        public string PublicId { get; set; }
        public string SecureUrl { get; set; }
    }
}
