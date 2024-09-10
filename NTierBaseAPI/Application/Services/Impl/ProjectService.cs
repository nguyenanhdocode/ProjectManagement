using Application.Exceptions;
using Application.Helpers;
using Application.Models.Asset;
using Application.Models.Project;
using AutoMapper;
using AutoMapper.Configuration.Conventions;
using Core.Entities;
using DataAccess.Repositories;
using DataAccess.UnifOfWork;
using System.Runtime.InteropServices;
using System.Transactions;

namespace Application.Services.Impl
{
    public class ProjectService : IProjectService
    {
        private IProjectRepository _projectRepository;
        private IMapper _mapper;
        private IClaimService _claimService;
        private IUnitOfWork _uow;
        private IAssetService _assetService;

        public ProjectService(IProjectRepository projectRepo
            , IMapper mapper
            , IClaimService claimService
            , IUnitOfWork unitOfWork
            , IAssetService assetService)
        {
            _projectRepository = projectRepo;
            _mapper = mapper;
            _claimService = claimService;
            _uow = unitOfWork;
            _assetService = assetService;
        }

        public async Task AddProjectAsset(AddProjectAssetModel model)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeOption.Required
                , new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }
                , TransactionScopeAsyncFlowOption.Enabled))
                {
                    var project = await _uow.ProjectRepository.GetByIdAsync(model.ProjectId);

                    if (project == null)
                        throw new BadRequestException("Project not found!");

                    var uploadResult = await _assetService.Upload(new Models.Asset.CreateAssetModel
                    {
                        File = model.File,
                    });

                    project.ProjectAssets = new List<ProjectAsset>()
                    {
                        new ProjectAsset
                        {
                            ProjectId = model.ProjectId,
                            AssetId = Guid.Parse(uploadResult.Id)
                        }
                    };

                    _uow.ProjectRepository.Update(project);
                    await _uow.SaveChangesAsync();

                    tran.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CreateProjectResponseModel> Create(CreateProjectModel model)
        {
            string id = UniqueIdHelper.GenerateId(11);
            while (await _projectRepository.GetByIdAsync(id) != null)
            {
                id = UniqueIdHelper.GenerateId(11);
            }

            var project = _mapper.Map<Project>(model);
            project.Id = id;
            project.CreatedUserId = _claimService.GetUserId();
            project.ManagerId = _claimService.GetUserId();

            await _uow.ProjectRepository.Add(project);
            await _uow.SaveChangesAsync();

            return new CreateProjectResponseModel { Id = id };
        }

        public async Task<List<ViewAssetModel>> GetAssetsOfProject(string projectId)
        {
            var assets = await _projectRepository.GetAssetsOfProject(projectId);
            return _mapper.Map<List<ViewAssetModel>>(assets);
        }

        public async Task RemoveAssetOfProject(RemoveAssetOfProjectModel model)
        {
            await _projectRepository.RemoveAssetOfProject(model.ProjectId, model.AssetId);

            if (!model.KeepAsset)
            {
                await _assetService.DeleteAsset(model.AssetId);
            }

            await _uow.SaveChangesAsync();
        }

        public async Task Update(UpdateProjectModel model)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(model.Id);

            if (project == null)
                throw new NotFoundException("Project not found");

            if (!project.CreatedUserId.Equals(_claimService.GetUserId()))
                throw new ForbiddenException();

            project.Name = model.Body.Name;
            project.Description = model.Body.Description;
            project.BeginDate = model.Body.BeginDate;
            project.EndDate = model.Body.EndDate;

            _uow.ProjectRepository.Update(project);
            await _uow.SaveChangesAsync();
        }
    }
}
