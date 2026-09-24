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

            if (!await context.Salas.AnyAsync())
            {
                context.Salas.AddRange(
                    new Sala { Nome = "Rubi", Capacidade = 500 },
                    new Sala { Nome = "Esmeralda", Capacidade = 150 },
                    new Sala { Nome = "Gold", Capacidade = 200 },
                    new Sala { Nome = "Safira", Capacidade = 250 },
                    new Sala { Nome = "Silver", Capacidade = 50 });

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

            if (!await context.Usuarios.AnyAsync(u => u.Email == adminEmail))
            {
                context.Usuarios.Add(new Usuario
                {
                    Nome = "Administrator",
                    Email = adminEmail,
                    Role = "admin",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword(adminPassword)
                });

                await context.SaveChangesAsync();
                logger.LogInformation("Seeded demo admin user {Email}.", adminEmail);
            }
        }
    }
}
