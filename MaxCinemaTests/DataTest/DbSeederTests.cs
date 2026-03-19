using MaxCinema.Data;
using MaxCinema.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaxCinemaTests.DataTest
{
    public class DbSeederTests
    {
        private ApplicationDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

     

        [Test]
        public async Task SeedSeatsAsync_CreatesSeats_WhenHallsExist()
        {
            
            _context.Halls.Add(new Hall { Id = 1, Name = "Зала 1", TotalRows = 2, SeatsPerRow = 3 });
            await _context.SaveChangesAsync();

            var services = CreateServiceProvider();

         
            await DbSeeder.SeedSeatsAsync(services);

            
            Assert.That(_context.Seats.Count(), Is.EqualTo(6));
        }

        [Test]
        public async Task SeedSeatsAsync_DoesNotCreateSeats_WhenAlreadyExist()
        {
           
            _context.Halls.Add(new Hall { Id = 1, Name = "Зала 1", TotalRows = 2, SeatsPerRow = 3 });
            _context.Seats.Add(new Seat { Id = 1, HallId = 1, Row = 1, Number = 1 });
            await _context.SaveChangesAsync();

            var services = CreateServiceProvider();

           
            await DbSeeder.SeedSeatsAsync(services);
            await DbSeeder.SeedSeatsAsync(services);

          
            Assert.That(_context.Seats.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task SeedSeatsAsync_CreatesCorrectRowsAndNumbers()
        {
           
            _context.Halls.Add(new Hall { Id = 1, Name = "Зала 1", TotalRows = 2, SeatsPerRow = 2 });
            await _context.SaveChangesAsync();

            var services = CreateServiceProvider();

          
            await DbSeeder.SeedSeatsAsync(services);

            var seats = _context.Seats.ToList();
            Assert.That(seats.Count, Is.EqualTo(4));
            Assert.That(seats.Any(s => s.Row == 1 && s.Number == 1), Is.True);
            Assert.That(seats.Any(s => s.Row == 1 && s.Number == 2), Is.True);
            Assert.That(seats.Any(s => s.Row == 2 && s.Number == 1), Is.True);
            Assert.That(seats.Any(s => s.Row == 2 && s.Number == 2), Is.True);
        }

        [Test]
        public async Task SeedSeatsAsync_NoHalls_CreatesNoSeats()
        {
           
            var services = CreateServiceProvider();

          
            await DbSeeder.SeedSeatsAsync(services);

           
            Assert.That(_context.Seats.Count(), Is.EqualTo(0));
        }
    

        [Test]
        public async Task SeedSeatsAsync_SetsCorrectHallId()
        {
           
            _context.Halls.Add(new Hall { Id = 1, Name = "Зала 1", TotalRows = 1, SeatsPerRow = 2 });
            await _context.SaveChangesAsync();

            var services = CreateServiceProvider();

            
            await DbSeeder.SeedSeatsAsync(services);

           
            var seats = _context.Seats.ToList();
            Assert.That(seats.All(s => s.HallId == 1), Is.True);
        }

       
        private IServiceProvider CreateServiceProvider()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddSingleton(_context);
            return serviceCollection.BuildServiceProvider();
        }
    }
}
