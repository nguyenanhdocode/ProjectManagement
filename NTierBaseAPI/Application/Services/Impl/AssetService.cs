using Application.Common;
using Application.Exceptions;
using Application.Models.Asset;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Core.Entities;
using DataAccess.UnifOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Application.Services.Impl
{
    public class AssetService : IAssetService
    {
        private Common.CloudinaryConfiguration _cloudinaryConfiguration;
        private Cloudinary cloudinary;
        private IUnitOfWork _uow;
        private IClaimService _claimService;
        private IMapper _mapper;

        public AssetService(IOptions<Common.CloudinaryConfiguration> cloudinaryConfiguration
            , IUnitOfWork unitOfWork
            , IClaimService claimService
            , IMapper mapper)
        {
            _cloudinaryConfiguration = cloudinaryConfiguration.Value;
            cloudinary = new Cloudinary(_cloudinaryConfiguration.Url);
            cloudinary.Api.Secure = true;
            _uow = unitOfWork;
            _claimService = claimService;
            _mapper = mapper;
        }

        public async Task DeleteAsset(string publicId)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeOption.Required
                    , new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }
                    , TransactionScopeAsyncFlowOption.Enabled))
                {
                    var asset = await _uow.AssetRepository.Get(p => p.AssetId.Equals(publicId));
                    if (asset == null)
                        throw new NotFoundException("Asset not found");

                    if (asset.CreatedUserId != _claimService.GetUserId())
                        throw new ForbiddenException();

                    _uow.AssetRepository.Delete(asset);
                    await _uow.SaveChangesAsync();

                    var deletionParams = new DeletionParams(publicId)
                    {
                        ResourceType = ResourceType.Auto
                    };
                    var deletionResult = await cloudinary.DeleteResourcesAsync(new string[] { publicId });

                    if (deletionResult.Error != null)
                        throw new Exception(deletionResult.Error.Message);

                    tran.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<AssetReponseModel>> GetAll()
        {
            string userId = _claimService.GetUserId();
            var result = await _uow.AssetRepository.GetManyAsync(p => p.CreatedUserId.Equals(userId));

            return _mapper.Map<List<AssetReponseModel>>(result);
        }

        public async Task<UploadResult> Upload(Stream stream)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(Guid.NewGuid().ToString(), stream)
            };
            return await cloudinary.UploadAsync(uploadParams);
        }

        public async Task<UploadImageResponseModel> UploadImage(UploadImageModel model)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeOption.Required
                    , new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }
                    , TransactionScopeAsyncFlowOption.Enabled))
                {
                    string assetId = Guid.NewGuid().ToString();
                    string fileFormat = Path.GetExtension(model.File.FileName);
                    var asset = new Asset
                    {
                        AssetId = assetId,
                        Path = $"{_cloudinaryConfiguration.ViewImageUrl}/{assetId}{fileFormat}",
                        CreatedUserId = _claimService.FindByKey(ClaimTypes.NameIdentifier),
                        Type = AssetTypes.Image
                    };

                    await _uow.AssetRepository.Add(asset);
                    await _uow.SaveChangesAsync();

                    using var stream = new MemoryStream();
                    await model.File.CopyToAsync(stream);
                    stream.Position = 0;

                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(assetId, stream),
                        PublicId = assetId,
                    };

                    ImageUploadResult uploadResult = await cloudinary.UploadAsync(uploadParams);


                    if (uploadResult.Error != null)
                        throw new Exception(uploadResult.Error.Message);

                    tran.Complete();

                    return new UploadImageResponseModel
                    {
                        PublicId = uploadResult.PublicId,
                        SecureUrl = asset.Path,
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
