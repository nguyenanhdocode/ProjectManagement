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
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(p => p.FirstName).IsRequired()
                .HasMaxLength(35)
                .HasColumnType("nvarchar");

            builder.Property(p => p.LastName).IsRequired()
                .HasMaxLength(35)
                .HasColumnType("nvarchar");

            builder.Property(p => p.Email).IsRequired();

            builder.Property(p => p.AvatarId).IsRequired(false);

            builder.Property(p => p.CreatedDate).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(p => p.Avatar).WithOne(p => p.AvatarUser);

            builder.Property(p => p.RefreshToken).IsRequired(false);

            builder.Property(p => p.RefreshTokenExpires).IsRequired(false);

            builder.Navigation(p => p.Avatar).AutoInclude();
        }
    }
}
