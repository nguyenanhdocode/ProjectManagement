using Core.Entities;
using DataAccess.UnifOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Impl
{
    public class ProjectRepository : BaseRepository<Project>, IProjectRepository
    {

        public async Task<List<Asset>> GetAssetsOfProject(string projectId)
        {
            var projectAsset = await Dbcontext.ProjectAssets.Include(p => p.Asset)
                .Where(p => p.ProjectId.Equals(projectId))
                .ToListAsync();

            return projectAsset.Select(p => p.Asset).ToList();
        }

        public async Task RemoveAssetOfProject(string projectId, Guid assetId)
        {
            var projectAsset = await Dbcontext.ProjectAssets
                .SingleOrDefaultAsync(p => p.ProjectId.Equals(projectId) && p.AssetId.Equals(assetId));

            if (projectAsset != null)
            {
                Dbcontext.ProjectAssets.Remove(projectAsset);
            }
        }

        public async Task RemoveAssetsOfProject(string projectId)
        {
            var projectAssets = await Dbcontext.ProjectAssets
                .Where(p => p.ProjectId.Equals(projectId))
                .ToListAsync();

            if (projectAssets != null)
            {
                Dbcontext.ProjectAssets.RemoveRange(projectAssets);
            }
        }
    }
}
