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
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .HasColumnType("varchar(11)");

            builder.Property(p => p.Name)
                .HasColumnType("nvarchar(256)")
                .IsRequired();

            builder.Property(p => p.BeginDate)
                .IsRequired();

            builder.Property(p => p.EndDate)
                .IsRequired();

            builder.Property(p => p.Description)
                .IsRequired(false)
                .HasColumnType("nvarchar(max)");

            builder.Property(p => p.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("getutcdate()");

            builder.Property(p => p.CreatedUserId)
                .IsRequired()
                .HasColumnType("nvarchar(450)");

            builder.Property(p => p.ManagerId)
                .IsRequired()
                .HasColumnType("nvarchar(450)");

            builder.HasOne(p => p.CreatedUser).WithMany(p => p.Projects).HasForeignKey(p => p.CreatedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Manager).WithMany(p => p.ManageProjects).HasForeignKey(p => p.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
