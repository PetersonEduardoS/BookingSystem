using Microsoft.EntityFrameworkCore;
using BookingSystem.Models;

namespace BookingSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Reserva> Reservas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configura os relacionamentos
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Room)
                .WithMany()
                .HasForeignKey(r => r.RoomId);
        }
    }
}
