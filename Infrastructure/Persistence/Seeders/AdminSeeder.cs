using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Seeders
{
    public class AdminSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Users.AnyAsync(u => u.Role == UserRole.Admin))
                return; // Admin already seeded

            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin@1234");

            var admin = User.Create(
                "Admin",
                "User",
                "admin@taskmanagement.com",
                passwordHash,
                UserRole.Admin);

            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
        }
    }
}
