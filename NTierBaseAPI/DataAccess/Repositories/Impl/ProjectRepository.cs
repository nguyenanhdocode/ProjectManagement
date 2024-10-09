using Core.Entities;
using DataAccess.UnifOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Impl
{
    public class ProjectRepository : BaseRepository<Project>, IProjectRepository
    {
        public void AddAssets(string projectId, List<string> assetIds)
        {
            foreach (var assetId in assetIds)
            {
                Dbcontext.ProjectAssets.Add(new ProjectAsset
                {
                    ProjectId = projectId,
                    AssetId = Guid.Parse(assetId)
                });
            }
        }

        public async Task<List<Asset>> GetAssetsOfProject(string projectId)
        {
            var projectAsset = await Dbcontext.ProjectAssets.Include(p => p.Asset)
                .Where(p => p.ProjectId.Equals(projectId))
                .ToListAsync();

            return projectAsset.Select(p => p.Asset).ToList();
        }

        public async Task<List<AppUser>> GetProjectMembers(string projectId)
        {
            return await Dbcontext.ProjectMembers.Where(p => p.ProjectId.Equals(projectId))
                .Select(p => p.Member)
                .ToListAsync();
        }

        public async Task<bool> IsAssetAdded(string projectId, string assetId)
        {
            var row = (await Dbcontext.ProjectAssets
                .SingleOrDefaultAsync(p => p.ProjectId.Equals(projectId) && p.AssetId.Equals(Guid.Parse(assetId))));

            return row != null;
        }

        public async Task<bool> IsUserJoinToProject(string projectId, string userId)
        {
            var projectMember = await Dbcontext.ProjectMembers
                .SingleOrDefaultAsync(p => p.ProjectId.Equals(projectId)
                && p.MemberId.Equals(userId));

            return projectMember != null;
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

        public async Task RemoveMember(string projectId, string memberId)
        {
            var row = await Dbcontext.ProjectMembers
                .SingleOrDefaultAsync(p => p.ProjectId.Equals(projectId) && p.MemberId.Equals(memberId));

            Dbcontext.RemoveRange(row);
        }
    }
}
