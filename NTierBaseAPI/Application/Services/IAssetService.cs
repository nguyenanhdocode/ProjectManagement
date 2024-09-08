using Application.Models.Asset;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IAssetService
    {
        /// <summary>
        /// Upload asset
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        Task<UploadResult> Upload(Stream stream);

        /// <summary>
        /// Upload image
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<UploadImageResponseModel> UploadImage(UploadImageModel model);

        /// <summary>
        /// Delete asset
        /// </summary>
        /// <param name="publicId"></param>
        /// <returns></returns>
        Task DeleteAsset(string publicId);

        /// <summary>
        /// Get all
        /// </summary>
        /// <returns></returns>
        Task<List<AssetReponseModel>> GetAll();
    }
}
