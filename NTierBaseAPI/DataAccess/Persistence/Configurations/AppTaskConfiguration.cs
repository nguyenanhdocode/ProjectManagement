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
    public class AppTaskConfiguration : IEntityTypeConfiguration<AppTask>
    {
        public void Configure(EntityTypeBuilder<AppTask> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnType("varchar(11)");
            builder.Property(p => p.Name).HasColumnType("nvarchar(256)").IsRequired();
            builder.Property(p => p.BeginDate).IsRequired();
            builder.Property(p => p.EndDate).IsRequired();
            builder.Property(p => p.Note).IsRequired(false).HasColumnType("nvarchar(max)");
            builder.Property(p => p.CreatedDate).IsRequired().HasDefaultValueSql("getutcdate()");
            builder.Property(p => p.CreatedUserId).IsRequired().HasColumnType("nvarchar(450)");
            builder.Property(p => p.AssignedToUserId).IsRequired(false).HasColumnType("nvarchar(450)");
            builder.Property(p => p.ProjectId).IsRequired().HasColumnType("varchar(11)");
            builder.Property(p => p.ParentId).IsRequired(false).HasColumnType("varchar(11)");
            builder.Property(p => p.PreviousTaskId).IsRequired(false).HasColumnType("varchar(11)");
            builder.Property(p => p.Status).IsRequired().HasDefaultValue(0);
            
            builder.HasCheckConstraint("CK_Task_Status", "Status >= 0 AND Status <= 3");

            builder.HasOne(p => p.CreatedUser).WithMany(p => p.AppTasks)
                .HasForeignKey(p => p.CreatedUserId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.AssignedToUser).WithMany(p => p.AssignedTasks)
                .HasForeignKey(p => p.AssignedToUserId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Project).WithMany(p => p.AppTasks)
                .HasForeignKey(p => p.ProjectId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Parent).WithMany(p => p.SubTasks)
                .HasForeignKey(p => p.ParentId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.PreviousTask).WithMany(p => p.NextTasks)
                .HasForeignKey(p => p.PreviousTaskId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
