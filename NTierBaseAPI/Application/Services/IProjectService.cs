using Application.Models.Asset;
using Application.Models.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IProjectService
    {
        /// <summary>
        /// Create project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<CreateProjectResponseModel> Create(CreateProjectModel model);

        /// <summary>
        /// Add asset to project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task AddProjectAsset(AddProjectAssetModel model);

        /// <summary>
        /// Get assets of project
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<List<ViewAssetModel>> GetAssetsOfProject(string projectId);

        /// <summary>
        /// Remove asset of project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task RemoveAssetOfProject(RemoveAssetOfProjectModel model);

        /// <summary>
        /// Update project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task Update(UpdateProjectModel model);
    }
}
