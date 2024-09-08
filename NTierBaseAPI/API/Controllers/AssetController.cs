using Application.Common;
using Application.Models;
using Application.Models.Asset;
using Application.Services;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Mozilla;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace API.Controllers
{
    [ApiController]
    [Route("api/assets")]
    public class AssetController : ControllerBase
    {
        private IAssetService _assetService;
        private IClaimService _claimService;
        private UserManager<AppUser> _userManager;

        public AssetController(IAssetService assetService
            , IClaimService claimService
            , IOptions<Application.Common.CloudinaryConfiguration> cloudinaryConfiguration
            , UserManager<AppUser> userManager)
        {
            _assetService = assetService;
            _claimService = claimService;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("images")]
        [Authorize]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageModel model)
        {
            return Ok(ApiResult<UploadImageResponseModel>.Success(await _assetService.UploadImage(model)));
        }

        [HttpDelete]
        [Route("images/{publicId}")]
        [Authorize]
        public async Task<IActionResult> DeleteAsset(string publicId)
        {
            await _assetService.DeleteAsset(publicId);
            return Ok(ApiResult<object?>.Success(null));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            return Ok(ApiResult<List<AssetReponseModel>>.Success(await _assetService.GetAll()));
        }

        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> Get()
        {
            AppUser user = await _userManager.FindByIdAsync(_claimService.GetUserId());
            return Ok(await _userManager.ChangeEmailAsync(user, "nguyen@gmail.com", "CfDJ8OilTr1Cv4BEiEZo9eDuea1kZrpo9e9uOiqs9IaoSNiXUxn2QVkHtmadzR4Fc+H+mh6Z9U6daYcmgmlrtnT/Eo9bcQ1MhzpNubvHvO9rYFFcaLe7qxRPEfSEmhOpCZHZGOpc95L924Yb2ykAWgXz/1cn588GSuf3bdXc5EwpRfvtZd2D27u0ErotC4dT6sw64LhXU5t+iOvE3sIBD/L9wKAOCxRDSnxW4DbfgCFhMUzpis3IZ7kb6EhHBUCFzBM/7w=="));
        }
    }
}
