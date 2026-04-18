using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Infrastructure.Persistence.Configurations
{

    public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            builder.ToTable("Tasks");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasMaxLength(2000);

            builder.Property(t => t.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(t => t.EstimatedHours)
                .HasColumnType("decimal(8,2)");

            builder.Property(t => t.LoggedHours)
                .HasColumnType("decimal(8,2)")
                .HasDefaultValue(0);

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            // Owner relationship
            builder.HasOne(t => t.Owner)
                .WithMany()
                .HasForeignKey(t => t.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Property(t => t.CreatedByUserId)
                    .IsRequired();

            // Project relationship (optional)
            builder.HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
            builder.HasOne(t => t.TaskType)
                .WithMany(tt => tt.Tasks)
                .HasForeignKey(t => t.TaskTypeId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // Index for common queries
            builder.HasIndex(t => t.OwnerId);
            builder.HasIndex(t => t.ProjectId);
            builder.HasIndex(t => t.Status);
        }
    }
}
