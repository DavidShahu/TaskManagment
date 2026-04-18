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
    public class ActiveTimerConfiguration : IEntityTypeConfiguration<ActiveTimer>
    {
        public void Configure(EntityTypeBuilder<ActiveTimer> builder)
        {
            builder.ToTable("ActiveTimers");
            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.Task)
                .WithMany()
                .HasForeignKey(t => t.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            // One active timer per user
            builder.HasIndex(t => t.UserId).IsUnique();
        }
    }
}
