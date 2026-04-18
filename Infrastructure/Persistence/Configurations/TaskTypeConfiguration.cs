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
    public class TaskTypeConfiguration : IEntityTypeConfiguration<TaskType>
    {
        public void Configure(EntityTypeBuilder<TaskType> builder)
        {
            builder.ToTable("TaskTypes");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Icon) 
                .HasMaxLength(10);

            builder.Property(t => t.Color) 
                .HasMaxLength(7); // hex color #ffffff

            builder.Property(t => t.IsActive)
                .HasDefaultValue(true);

            // Seed default task types
            builder.HasData(
                new
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Programming", 
                    Color = "#667eea",
                    IsActive = true
                },
                new
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Design", 
                    Color = "#ed64a6",
                    IsActive = true
                },
                new
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Bug Fix", 
                    Color = "#e53e3e",
                    IsActive = true
                },
                new
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Name = "Research", 
                    Color = "#48bb78",
                    IsActive = true
                },
                new
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Name = "Meeting", 
                    Color = "#ed8936",
                    IsActive = true
                }
            );
        }
    }
}
