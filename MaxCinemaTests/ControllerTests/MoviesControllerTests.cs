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
    public class MoviesControllerTests
    {
        private ApplicationDbContext _context;
        private MoviesController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

            _context = new ApplicationDbContext(options);
            _controller = new MoviesController(_context);
        }
        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
            _controller.Dispose();
        }
        [Test]
        public async Task Index_ReturnsViewResult()
        {
            var result = await _controller.Index();
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }
        [Test]
        public async Task Index_ReturnsAllMovies()
        {
            _context.Movies.AddRange(
                new Movie { Id = 1, Title = "Minions", Genre = "Decko", DurationMinutes = 140 },
                new Movie { Id = 2, Title = "The Dark Knight", Genre = "Action", DurationMinutes = 152 }
            );
            await _context.SaveChangesAsync();

            var result = await _controller.Index() as ViewResult;
            var model = result?.Model as IEnumerable<Movie>;

            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task Index_EmptyDatabase_ReturnsEmptyList()
        {
            var result = await _controller.Index() as ViewResult;
            var model = result?.Model as IEnumerable<Movie>;

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
        public async Task Details_ValidId_ReturnsViewWithMovie()
        {
            _context.Movies.Add(new Movie { Id = 1, Title = "Minions", Genre = "Decko", DurationMinutes = 140 });
            await _context.SaveChangesAsync();

            var result = await _controller.Details(1) as ViewResult;
            var model = result?.Model as Movie;

            Assert.That(model, Is.Not.Null);
            Assert.That(model.Title, Is.EqualTo("Minions"));
        }

        [Test]
        public void Create_Get_ReturnsViewResult()
        {
            var result = _controller.Create();
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task Create_Post_ValidMovie_RedirectsToIndex()
        {
            var movie = new Movie
            {
                Title = "Minions",
                Genre = "Decko",
                DurationMinutes = 140,
                Description = "A thriller",
                ImageUrl = "/images/movies/default.jpg",
                IsActive = true
            };

            var result = await _controller.Create(movie);
            var redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task Create_Post_ValidMovie_SavesToDatabase()
        {
            var movie = new Movie
            {
                Title = "Minions",
                Genre = "Decko",
                DurationMinutes = 140,
                Description = "A thriller",
                ImageUrl = "/images/movies/default.jpg",
                IsActive = true
            };

            await _controller.Create(movie);

            Assert.That(_context.Movies.Count(), Is.EqualTo(1));
            Assert.That(_context.Movies.First().Title, Is.EqualTo("Minions"));
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
        public async Task Edit_ValidId_ReturnsViewWithMovie()
        {
            _context.Movies.Add(new Movie { Id = 1, Title = "Minions", Genre = "Decko", DurationMinutes = 140 });
            await _context.SaveChangesAsync();

            var result = await _controller.Edit(1) as ViewResult;
            var model = result?.Model as Movie;

            Assert.That(model, Is.Not.Null);
            Assert.That(model.Title, Is.EqualTo("Minions"));
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
        public async Task Delete_ValidId_ReturnsViewWithMovie()
        {
            _context.Movies.Add(new Movie { Id = 1, Title = "Minions", Genre = "Decko", DurationMinutes = 140 });
            await _context.SaveChangesAsync();

            var result = await _controller.Delete(1) as ViewResult;
            var model = result?.Model as Movie;

            Assert.That(model, Is.Not.Null);
            Assert.That(model.Title, Is.EqualTo("Minions"));
        }

        [Test]
        public async Task DeleteConfirmed_RemovesMovie_AndRedirects()
        {
            _context.Movies.Add(new Movie { Id = 1, Title = "Minions", Genre = "Decko", DurationMinutes = 140 });
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteConfirmed(1);
            var redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(_context.Movies.Count(), Is.EqualTo(0));
        }
    }
}
