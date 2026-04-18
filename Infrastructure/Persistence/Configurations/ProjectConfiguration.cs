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
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects");

            builder.HasKey(x => x.Id);

            builder.Property(x=> x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x=> x.Description).HasMaxLength(500);
            builder.Property(p => p.CreatedAt).IsRequired();

            builder.Property(p => p.IsActive).IsRequired().HasDefaultValue(true);
            
            builder.HasOne(p => p.CreatedBy).WithMany().HasForeignKey(p => p.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
