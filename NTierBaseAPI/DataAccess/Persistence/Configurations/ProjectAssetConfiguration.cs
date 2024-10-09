using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Persistence.Configurations
{
    public class ProjectAssetConfiguration : IEntityTypeConfiguration<ProjectAsset>
    {
        public void Configure(EntityTypeBuilder<ProjectAsset> builder)
        {
            builder.HasKey(p => new { p.ProjectId, p.AssetId });

            builder.Property(p => p.ProjectId).HasColumnType("varchar(11)");

            builder.Property(p => p.AddedDate).IsRequired().HasDefaultValueSql("getutcdate()");

            builder.HasOne(p => p.Project).WithMany(p => p.ProjectAssets).HasForeignKey(p => p.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Asset).WithMany(p => p.ProjectAssets).HasForeignKey(p => p.AssetId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
