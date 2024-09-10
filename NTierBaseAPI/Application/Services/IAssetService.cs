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
        Task<CreateAssetResponseModel> Upload(CreateAssetModel model);

        /// <summary>
        /// Upload image
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<CreateAssetResponseModel> UploadImage(CreateImageModel model);

        /// <summary>
        /// Delete asset
        /// </summary>
        /// <param name="publicId"></param>
        /// <returns></returns>
        Task DeleteAsset(Guid publicId);

        /// <summary>
        /// Get all
        /// </summary>
        /// <returns></returns>
        Task<List<ViewAssetModel>> GetAll();
    }
}
