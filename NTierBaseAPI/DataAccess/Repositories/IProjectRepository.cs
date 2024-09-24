using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IProjectRepository : IBaseRepository<Project>
    {
        /// <summary>
        /// Get assets of project
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<List<Asset>> GetAssetsOfProject(string projectId);

        /// <summary>
        /// Remove asset of project
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="assetId"></param>
        /// <param name="keepFile"></param>
        /// <returns></returns>
        Task RemoveAssetOfProject(string projectId, Guid assetId);

        /// <summary>
        /// Remove assets of project
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task RemoveAssetsOfProject(string projectId);

        /// <summary>
        /// Check if is user join to project
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<bool> IsUserJoinToProject(string projectId, string userId);

        /// <summary>
        /// Get project members
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<List<AppUser>> GetProjectMembers(string projectId);
    }
}
