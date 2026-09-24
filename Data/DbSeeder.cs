using BookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(DbSeeder));

            await context.Database.MigrateAsync();

            if (!await context.Rooms.AnyAsync())
            {
                context.Rooms.AddRange(
                    new Room { Name = "Rubi", Capacity = 500 },
                    new Room { Name = "Esmeralda", Capacity = 150 },
                    new Room { Name = "Gold", Capacity = 200 },
                    new Room { Name = "Safira", Capacity = 250 },
                    new Room { Name = "Silver", Capacity = 50 });

                await context.SaveChangesAsync();
                logger.LogInformation("Seeded demo rooms.");
            }

            var adminEmail = configuration["SeedAdmin:Email"]?.Trim().ToLower();
            var adminPassword = configuration["SeedAdmin:Password"];

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            {
                logger.LogInformation("SeedAdmin not configured. Skipping admin seed.");
                return;
            }

            if (!await context.Users.AnyAsync(u => u.Email == adminEmail))
            {
                context.Users.Add(new User
                {
                    Name = "Administrator",
                    Email = adminEmail,
                    Role = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword)
                });

                await context.SaveChangesAsync();
                logger.LogInformation("Seeded demo admin user {Email}.", adminEmail);
            }
        }
    }
}
