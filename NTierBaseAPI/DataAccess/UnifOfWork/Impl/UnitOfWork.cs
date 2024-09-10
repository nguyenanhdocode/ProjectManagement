using Core.Common;
using Core.Entities;
using DataAccess.Persistence;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

namespace DataAccess.UnifOfWork.Impl
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppDbContext DbContext { get; }

        public IAssetRepository AssetRepository { get; set; }
        public IProjectRepository ProjectRepository { get; set; }

        public UnitOfWork(AppDbContext dbContext
            , IAssetRepository assetRepo
            , IProjectRepository projectRepository
            , IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            DbContext = dbContext;

            AssetRepository = assetRepo;
            AssetRepository.Dbcontext = DbContext;

            ProjectRepository = projectRepository;
            ProjectRepository.Dbcontext = DbContext;
        }

        public async Task<int> SaveChangesAsync()
        {
            string? userId = _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            foreach (var entry in DbContext.ChangeTracker.Entries<IAuditedEntity>())
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedUserId = userId;
                        entry.Entity.CreatedOn = DateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedUserId = userId;
                        entry.Entity.UpdatedOn = DateTime.Now;
                        break;
                }
            return await DbContext.SaveChangesAsync();
        }
    }
}
