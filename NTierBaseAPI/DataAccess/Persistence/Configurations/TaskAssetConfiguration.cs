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
    public class TaskAssetConfiguration : IEntityTypeConfiguration<TaskAsset>
    {
        public void Configure(EntityTypeBuilder<TaskAsset> builder)
        {
            builder.HasKey(p => new { p.TaskId, p.AssetId });
            builder.Property(p => p.TaskId).HasColumnType("varchar(11)");

            builder.HasOne(p => p.Task).WithMany(p => p.TaskAssets)
                .HasForeignKey(p => p.TaskId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Asset).WithMany(p => p.TaskAssets)
                .HasForeignKey(p => p.AssetId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
