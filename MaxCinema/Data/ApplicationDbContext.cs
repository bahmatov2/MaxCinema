using MaxCinema.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MaxCinema.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>  // ← ПРОМЕНИХ ТОВА
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Projection> Projections { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ПЪРВО - всички връзки с Restrict
            modelBuilder.Entity<Projection>()
                .HasOne(p => p.Hall)
                .WithMany(h => h.Projections)
                .HasForeignKey(p => p.HallId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Projection>()
                .HasOne(p => p.Movie)
                .WithMany(m => m.Projections)
                .HasForeignKey(p => p.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Hall)
                .WithMany(h => h.Seats)
                .HasForeignKey(s => s.HallId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Projection)
                .WithMany(p => p.Tickets)
                .HasForeignKey(t => t.ProjectionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Seat)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Индекс
            modelBuilder.Entity<Ticket>()
                .HasIndex(t => new { t.ProjectionId, t.SeatId })
                .IsUnique();

            // Seed data
            modelBuilder.Entity<Hall>().HasData(
                new Hall { Id = 1, Name = "Зала 1", TotalRows = 8, SeatsPerRow = 10 },
                new Hall { Id = 2, Name = "Зала 2", TotalRows = 6, SeatsPerRow = 8 },
                new Hall { Id = 3, Name = "Зала 3", TotalRows = 6, SeatsPerRow = 8 },
                new Hall { Id = 4, Name = "Зала 4", TotalRows = 6, SeatsPerRow = 8 },
                new Hall { Id = 5, Name = "Зала 5", TotalRows = 6, SeatsPerRow = 8 }
            );
        }
    }
}