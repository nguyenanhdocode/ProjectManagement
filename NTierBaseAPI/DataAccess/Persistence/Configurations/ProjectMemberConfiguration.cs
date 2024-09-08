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
    public class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
    {
        public void Configure(EntityTypeBuilder<ProjectMember> builder)
        {
            builder.HasKey(p => new { p.ProjectId, p.MemberId });

            builder.Property(p => p.ProjectId).HasColumnType("varchar(11)");
            builder.Property(p => p.MemberId).HasColumnType("nvarchar(450)");

            builder.HasOne(p => p.Project).WithMany(p => p.ProjectMembers).HasForeignKey(p => p.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Member).WithMany(p => p.ProjectMembers).HasForeignKey(p => p.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
