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
    public class AssetConfiguration : IEntityTypeConfiguration<Asset>
    {
        public void Configure(EntityTypeBuilder<Asset> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasDefaultValue(Guid.NewGuid());
            builder.Property(p => p.Path).IsRequired();
            builder.Property(p => p.AssetId).IsRequired();
            builder.Property(p => p.CreatedUserId).HasMaxLength(450).IsRequired(false);
            builder.Property(p => p.CreatedDate).HasDefaultValue(DateTime.UtcNow);
            builder.Property(p => p.Type).IsRequired().HasColumnType("varchar(100)");

            builder.HasOne(p => p.CreatedUser).WithMany(p => p.Assets).HasForeignKey(p => p.CreatedUserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
