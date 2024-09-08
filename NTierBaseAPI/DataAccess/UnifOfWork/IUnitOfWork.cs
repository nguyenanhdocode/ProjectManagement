using Core.Entities;
using DataAccess.Persistence;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.UnifOfWork
{
    public interface IUnitOfWork
    {
        public IBaseRepository<Asset> AssetRepository { get; set; }
        AppDbContext DbContext { get; }
        Task<int> SaveChangesAsync();
    }
}
