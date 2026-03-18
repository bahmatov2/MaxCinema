using MaxCinema.Controllers;
using MaxCinema.Data;
using MaxCinema.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaxCinemaTests.ControllerTests
{
    public class HallControllerTests
    {
        public ApplicationDbContext _context;
        public HallsController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new HallsController(_context);
        }
        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
            _context.Dispose();
        }
        [Test]
        public async Task Index_ReturnsViewResult()
        {
            var result = await _controller.Index();
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task Index_ReturnsAllHalls()
        {
            
            _context.Halls.AddRange(
                new Hall { Id = 1, Name = "Зала 1", TotalRows = 8, SeatsPerRow = 10 },
                new Hall { Id = 2, Name = "Зала 2", TotalRows = 6, SeatsPerRow = 8 }
            );
            await _context.SaveChangesAsync();

            
            var result = await _controller.Index() as ViewResult;
            var model = result?.Model as IEnumerable<Hall>;

           
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task Index_EmptyDatabase_ReturnsEmptyList()
        {
            var result = await _controller.Index() as ViewResult;
            var model = result?.Model as IEnumerable<Hall>;

            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count(), Is.EqualTo(0));
        }

        
        [Test]
        public async Task Details_NullId_ReturnsNotFound()
        {
            var result = await _controller.Details(null);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            var result = await _controller.Details(999);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Details_ValidId_ReturnsViewWithHall()
        {
           
            _context.Halls.Add(new Hall { Id = 1, Name = "Зала 1", TotalRows = 8, SeatsPerRow = 10 });
            await _context.SaveChangesAsync();

          
            var result = await _controller.Details(1) as ViewResult;
            var model = result?.Model as Hall;

          
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Name, Is.EqualTo("Зала 1"));
        }

       
        [Test]
        public void Create_Get_ReturnsViewResult()
        {
            var result = _controller.Create();
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task Create_Post_ValidHall_RedirectsToIndex()
        {
         
            var hall = new Hall { Name = "Зала 1", TotalRows = 8, SeatsPerRow = 10 };

          
            var result = await _controller.Create(hall);
            var redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task Create_Post_ValidHall_SavesToDatabase()
        {
          
            var hall = new Hall { Name = "Зала 1", TotalRows = 8, SeatsPerRow = 10 };

            
            await _controller.Create(hall);

          
            Assert.That(_context.Halls.Count(), Is.EqualTo(1));
            Assert.That(_context.Halls.First().Name, Is.EqualTo("Зала 1"));
        }

        [Test]
        public async Task Edit_NullId_ReturnsNotFound()
        {
            var result = await _controller.Edit(null);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Edit_InvalidId_ReturnsNotFound()
        {
            var result = await _controller.Edit(999);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Edit_ValidId_ReturnsViewWithHall()
        {
            
            _context.Halls.Add(new Hall { Id = 1, Name = "Зала 1", TotalRows = 8, SeatsPerRow = 10 });
            await _context.SaveChangesAsync();

         
            var result = await _controller.Edit(1) as ViewResult;
            var model = result?.Model as Hall;

            Assert.That(model, Is.Not.Null);
            Assert.That(model.Name, Is.EqualTo("Зала 1"));
        }

        
        [Test]
        public async Task Delete_NullId_ReturnsNotFound()
        {
            var result = await _controller.Delete(null);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Delete_InvalidId_ReturnsNotFound()
        {
            var result = await _controller.Delete(999);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Delete_ValidId_ReturnsViewWithHall()
        {
           
            _context.Halls.Add(new Hall { Id = 1, Name = "Зала 1", TotalRows = 8, SeatsPerRow = 10 });
            await _context.SaveChangesAsync();

            
            var result = await _controller.Delete(1) as ViewResult;
            var model = result?.Model as Hall;

           
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Name, Is.EqualTo("Зала 1"));
        }

        [Test]
        public async Task DeleteConfirmed_RemovesHall_AndRedirects()
        {
           
            _context.Halls.Add(new Hall { Id = 1, Name = "Зала 1", TotalRows = 8, SeatsPerRow = 10 });
            await _context.SaveChangesAsync();

            
            var result = await _controller.DeleteConfirmed(1);
            var redirect = result as RedirectToActionResult;

            
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(_context.Halls.Count(), Is.EqualTo(0));
        }

        
        [Test]
        public void GenerateSeats_CreatesSeatsForAllHalls()
        {
            
            _context.Halls.Add(new Hall { Id = 1, Name = "Зала 1", TotalRows = 2, SeatsPerRow = 3 });
            _context.SaveChanges();

          
            _controller.GenerateSeeds();        
            Assert.That(_context.Seats.Count(), Is.EqualTo(6));
        }

        [Test]
        public void GenerateSeats_DoesNotDuplicateSeats()
        {
            
            _context.Halls.Add(new Hall { Id = 1, Name = "Зала 1", TotalRows = 2, SeatsPerRow = 3 });
            _context.SaveChanges();

           
            _controller.GenerateSeeds();
            _controller.GenerateSeeds();

            Assert.That(_context.Seats.Count(), Is.EqualTo(6));
        }
    }
}

