using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Impl
{
    public class AssetRepository : BaseRepository<Asset>, IAssetRepository
    {
        public async Task RemoveWithReferences(Guid id)
        {
            var asset = await Dbcontext.Assets
                .Include(p => p.ProjectAssets)
                .Include(p => p.TaskAssets)
                .SingleOrDefaultAsync(p => p.Id.Equals(id));

            if (asset != null)
            {
                asset.ProjectAssets.Clear();
                Dbcontext.Assets.Update(asset);

                asset.TaskAssets.Clear();
                Dbcontext.Assets.Update(asset);

                Dbcontext.Assets.Remove(asset);
            }
        }
    }
}
