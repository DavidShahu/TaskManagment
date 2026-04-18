using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{

    public class TimeLogConfiguration : IEntityTypeConfiguration<TimeLog>
    {
        public void Configure(EntityTypeBuilder<TimeLog> builder)
        {
            builder.ToTable("TimeLogs");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Hours)
                .HasColumnType("decimal(8,2)")
                .IsRequired();

            builder.Property(t => t.Note)
                .HasMaxLength(500);

            builder.Property(t => t.LoggedAt)
                .IsRequired();

            // Task relationship
            builder.HasOne(t => t.Task)
                .WithMany(t => t.TimeLogs)
                .HasForeignKey(t => t.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            // User relationship
            builder.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(t => t.TaskId);
            builder.HasIndex(t => t.UserId);
        }
    }
}
