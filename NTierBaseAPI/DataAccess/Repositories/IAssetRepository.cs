using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IAssetRepository : IBaseRepository<Asset>
    {
        /// <summary>
        /// Remove asset with references
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task RemoveWithReferences(Guid id);
    }
}
