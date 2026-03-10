using MaxCinema.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MaxCinema.Data
{
    public class ApplicationDbContext : IdentityDbContext
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

            
            modelBuilder.Entity<Ticket>()
                .HasIndex(t => new { t.ProjectionId, t.SeatId })
                .IsUnique();

            //залите
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

