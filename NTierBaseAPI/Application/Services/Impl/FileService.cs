using Application.Common;
using Application.Exceptions;
using Application.Models.Asset;
using AutoMapper;
using Core.Entities;
using DataAccess.UnifOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Transactions;

namespace Application.Services.Impl
{
    public class FileService : IAssetService
    {
        private IClaimService _claimService;
        private IUnitOfWork _uow;
        private IMapper _mapper;
        private StaticConfiguration _staticConfiguration;
        private UserManager<AppUser> _userManager;

        public FileService(IClaimService claimService
            , IUnitOfWork unitOfWork
            , IMapper mapper
            , IOptions<StaticConfiguration> staticConfiguration
            , UserManager<AppUser> userManager)
        {
            _claimService = claimService;
            _uow = unitOfWork;
            _mapper = mapper;
            _staticConfiguration = staticConfiguration.Value;
            _userManager = userManager;
        }

        public async Task DeleteAsset(Guid id)
        {
            var file = await _uow.AssetRepository.GetByIdAsync(id);

            if (file == null)
                throw new NotFoundException("File not found!");

            if (file.CreatedUserId != _claimService.GetUserId())
                throw new ForbiddenException();

            try
            {
                using (var tran = new TransactionScope(TransactionScopeOption.Required
                , new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }
                , TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _uow.AssetRepository.RemoveWithReferences(id);
                    await _uow.SaveChangesAsync();

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), file.RelativePath);
                    File.Delete(filePath);

                    tran.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ViewAssetModel>> GetAll()
        {
            string userId = _claimService.GetUserId();
            var files = await _uow.AssetRepository.GetManyAsync(p => p.CreatedUserId.Equals(userId));

            var results = _mapper.Map<List<ViewAssetModel>>(files);

            return results;
        }

        public async Task<CreateAssetResponseModel> Upload(CreateAssetModel model)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeOption.Required
                    , new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }
                    , TransactionScopeAsyncFlowOption.Enabled))
                {
                    string fileId = model.CustomId ?? Guid.NewGuid().ToString();
                    string fileName = string.Format("{0}{1}", fileId, Path.GetExtension(model.File.FileName));
                    string relativePath = Path.Combine(_staticConfiguration.BaseDir
                        , _staticConfiguration.UploadDir, fileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

                    var asset = new Asset
                    {
                        Id = Guid.Parse(fileId),
                        AssetId = fileId,
                        RelativePath = relativePath,
                        Type = Path.GetExtension(model.File.FileName).ToLower().Remove(0, 1),
                        FileName = fileName,
                        CreatedUserId = _claimService.GetUserId(),
                        Size = model.File.Length,
                        DisplayFileName = model.File.FileName,
                    };

                    await _uow.AssetRepository.Add(asset);
                    await _uow.SaveChangesAsync();

                    using (var fileSrteam = new FileStream(filePath, FileMode.Create))
                    {
                        await model.File.CopyToAsync(fileSrteam);
                    }

                    tran.Complete();

                    return _mapper.Map<CreateAssetResponseModel>(asset);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CreateAssetResponseModel> UploadImage(CreateImageModel model)
        {
            return await Upload(model);
        }
    }
}
