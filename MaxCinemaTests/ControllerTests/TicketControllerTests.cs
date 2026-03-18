using MaxCinema.Controllers;
using MaxCinema.Data;
using MaxCinema.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace MaxCinemaTests.ControllerTests
{
    public class TicketControllerTests
    {
        public ApplicationDbContext _context;
        public TicketsController _controller;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            _controller = new TicketsController(_context, _userManagerMock.Object);


            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Name, "test@test.com"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

        }
        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
            _context.Dispose();
        }

        [Test]
        public async Task Index_AdminUser_ReturnsViewWithAllTickets()
        {
            var user = new ApplicationUser { Id = "test-user-id", UserName = "test@test.com" };
            var movie = new Movie { Id = 1, Title = "Test Movie 1" };
            var hall = new Hall { Id = 1, Name = "Test Hall 1", TotalRows = 10, SeatsPerRow = 10 };
            var seat1 = new Seat { Id = 1, Row = 1, Number = 1, HallId = 1 };
            var seat2 = new Seat { Id = 2, Row = 1, Number = 2, HallId = 1 };

            var projection = new Projection
            {
                Id = 1,
                MovieId = 1,
                HallId = 1,
                StartTime = DateTime.Now.AddDays(1),
                TicketPrice = 12.50m
            };

            var tickets = new List<Ticket>
            {
                new Ticket
                {
                    Id = 1,
                    ProjectionId = 1,
                    SeatId = 1,
                    UserId = "test-user-id",
                    PricePaid = 12.50m,
                    PurchasedAt = DateTime.Now.AddHours(-1)
                },
                new Ticket
                {
                    Id = 2,
                    ProjectionId = 1,
                    SeatId = 2,
                    UserId = "test-user-id",
                    PricePaid = 12.50m,
                    PurchasedAt = DateTime.Now.AddMinutes(-30)
                }
            };
            _context.Users.Add(user);
            _context.Movies.Add(movie);
            _context.Halls.Add(hall);
            _context.Seats.AddRange(seat1, seat2);
            _context.Projections.Add(projection);
            _context.Tickets.AddRange(tickets);
            await _context.SaveChangesAsync();


            var result = await _controller.Index() as ViewResult;
            var model = result?.Model as List<Ticket>;


            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count, Is.EqualTo(2));


            var firstTicket = model.First();
            Assert.That(firstTicket.Projection, Is.Not.Null);
            Assert.That(firstTicket.Projection.Movie, Is.Not.Null);
            Assert.That(firstTicket.Projection.Movie.Title, Is.EqualTo("Test Movie 1"));
            Assert.That(firstTicket.Projection.Hall, Is.Not.Null);
            Assert.That(firstTicket.Projection.Hall.Name, Is.EqualTo("Test Hall 1"));
            Assert.That(firstTicket.Seat, Is.Not.Null);
            Assert.That(firstTicket.Seat.Row, Is.EqualTo(1));
        }

        [Test]

        public async Task MyTickets_ReturnsOnlyCurrentUserTickets()
        {
          
            var currentUserId = "test-user-id";
            var otherUserId = "other-user-id";

           
            var currentUser = new ApplicationUser { Id = currentUserId, UserName = "current@test.com" };
            var otherUser = new ApplicationUser { Id = otherUserId, UserName = "other@test.com" };

            _context.Users.AddRange(currentUser, otherUser);

           
            var movie = new Movie { Id = 1, Title = "Test Movie" };
            var hall = new Hall { Id = 1, Name = "Test Hall", TotalRows = 10, SeatsPerRow = 10 };

            _context.Movies.Add(movie);
            _context.Halls.Add(hall);

            
            var projection = new Projection
            {
                Id = 1,
                MovieId = 1,
                HallId = 1,
                StartTime = DateTime.Now.AddDays(1),
                TicketPrice = 12.50m
            };
            _context.Projections.Add(projection);

            
            var seats = new List<Seat>();
            for (int i = 1; i <= 5; i++)
            {
                seats.Add(new Seat { Id = i, Row = 1, Number = i, HallId = 1 });
            }
            _context.Seats.AddRange(seats);

          
            var currentUserTickets = new List<Ticket>
    {
        new Ticket
        {
            Id = 1,
            ProjectionId = 1,
            SeatId = 1,
            UserId = currentUserId,
            PricePaid = 12.50m,
            PurchasedAt = DateTime.Now.AddHours(-3)
        },
        new Ticket
        {
            Id = 2,
            ProjectionId = 1,
            SeatId = 2,
            UserId = currentUserId,
            PricePaid = 12.50m,
            PurchasedAt = DateTime.Now.AddHours(-2)
        },
        new Ticket
        {
            Id = 3,
            ProjectionId = 1,
            SeatId = 3,
            UserId = currentUserId,
            PricePaid = 12.50m,
            PurchasedAt = DateTime.Now.AddHours(-1)
        }
    };

           
            var otherUserTickets = new List<Ticket>
    {
        new Ticket
        {
            Id = 4,
            ProjectionId = 1,
            SeatId = 4,
            UserId = otherUserId,
            PricePaid = 12.50m,
            PurchasedAt = DateTime.Now.AddHours(-4)
        },
        new Ticket
        {
            Id = 5,
            ProjectionId = 1,
            SeatId = 5,
            UserId = otherUserId,
            PricePaid = 12.50m,
            PurchasedAt = DateTime.Now.AddHours(-5)
        }
    };

            _context.Tickets.AddRange(currentUserTickets);
            _context.Tickets.AddRange(otherUserTickets);
            await _context.SaveChangesAsync();

           
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, currentUserId),
        new Claim(ClaimTypes.Name, "current@test.com"),
        new Claim(ClaimTypes.Role, "User")
    }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            
            var result = await _controller.MyTickets() as ViewResult;
            var model = result?.Model as List<Ticket>;

           
            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count, Is.EqualTo(3)); 

            foreach (var ticket in model)
            {
                Assert.That(ticket.UserId, Is.EqualTo(currentUserId));
            }

           
            Assert.That(model.Any(t => t.UserId == otherUserId), Is.False);

           
            var firstTicket = model.First();
            Assert.That(firstTicket.Projection, Is.Not.Null);
            Assert.That(firstTicket.Projection.Movie, Is.Not.Null);
            Assert.That(firstTicket.Projection.Hall, Is.Not.Null);
            Assert.That(firstTicket.Seat, Is.Not.Null);
        }
        [Test]
        public async Task Edit_Get_AdminUser_ReturnsViewWithTicket()
        {
            
            var user = new ApplicationUser { Id = "test-user-id", UserName = "test@test.com" };
            var movie = new Movie { Id = 1, Title = "Test Movie" };
            var hall = new Hall { Id = 1, Name = "Test Hall", TotalRows = 10, SeatsPerRow = 10 };
            var seat = new Seat { Id = 1, Row = 1, Number = 1, HallId = 1 };

            var projection = new Projection
            {
                Id = 1,
                MovieId = 1,
                HallId = 1,
                StartTime = DateTime.Now.AddDays(1),
                TicketPrice = 12.50m
            };

            var ticket = new Ticket
            {
                Id = 1,
                ProjectionId = 1,
                SeatId = 1,
                UserId = "test-user-id",
                PricePaid = 12.50m,
                PurchasedAt = DateTime.Now.AddHours(-1)
            };

           
            _context.Users.Add(user);
            _context.Movies.Add(movie);
            _context.Halls.Add(hall);
            _context.Seats.Add(seat);
            _context.Projections.Add(projection);
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            
            var result = await _controller.Edit(1) as ViewResult;
            var model = result?.Model as Ticket;

            
            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Id, Is.EqualTo(1));
            Assert.That(model.PricePaid, Is.EqualTo(12.50m));
            Assert.That(model.ProjectionId, Is.EqualTo(1));
            Assert.That(model.SeatId, Is.EqualTo(1));
            Assert.That(model.UserId, Is.EqualTo("test-user-id"));

         
            Assert.That(result.ViewData["ProjectionId"], Is.InstanceOf<SelectList>());
            Assert.That(result.ViewData["SeatId"], Is.InstanceOf<SelectList>());
            Assert.That(result.ViewData["UserId"], Is.InstanceOf<SelectList>());
        }
        [Test]
        public async Task Details_UserTryingToSeeOthersTicket_ReturnsUnauthorized()
        {
           
            var currentUserId = "test-user-id";    
            var otherUserId = "other-user-id";      

            
            var currentUser = new ApplicationUser { Id = currentUserId, UserName = "current@test.com" };
            var otherUser = new ApplicationUser { Id = otherUserId, UserName = "other@test.com" };

            _context.Users.AddRange(currentUser, otherUser);

            
            var movie = new Movie { Id = 1, Title = "Test Movie" };
            var hall = new Hall { Id = 1, Name = "Test Hall", TotalRows = 10, SeatsPerRow = 10 };
            var seat = new Seat { Id = 1, Row = 1, Number = 1, HallId = 1 };

            var projection = new Projection
            {
                Id = 1,
                MovieId = 1,
                HallId = 1,
                StartTime = DateTime.Now.AddDays(1),
                TicketPrice = 12.50m
            };

           
            var otherTicket = new Ticket
            {
                Id = 1,
                ProjectionId = 1,
                SeatId = 1,
                UserId = otherUserId, 
                PricePaid = 12.50m,
                PurchasedAt = DateTime.Now.AddHours(-1)
            };

            _context.Movies.Add(movie);
            _context.Halls.Add(hall);
            _context.Seats.Add(seat);
            _context.Projections.Add(projection);
            _context.Tickets.Add(otherTicket);
            await _context.SaveChangesAsync();

           
            var nonAdminUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, currentUserId),
        new Claim(ClaimTypes.Name, "current@test.com"),
        new Claim(ClaimTypes.Role, "User")  
    }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = nonAdminUser }
            };

            
            var result = await _controller.Details(1);

          
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task Details_AdminCanSeeAnyTicket_ReturnsView()
        {
           
            var adminUserId = "admin-user-id";
            var otherUserId = "other-user-id";

            
            var adminUser = new ApplicationUser { Id = adminUserId, UserName = "admin@test.com" };
            var otherUser = new ApplicationUser { Id = otherUserId, UserName = "other@test.com" };

            _context.Users.AddRange(adminUser, otherUser);

           
            var movie = new Movie { Id = 1, Title = "Test Movie" };
            var hall = new Hall { Id = 1, Name = "Test Hall", TotalRows = 10, SeatsPerRow = 10 };
            var seat = new Seat { Id = 1, Row = 1, Number = 1, HallId = 1 };

            var projection = new Projection
            {
                Id = 1,
                MovieId = 1,
                HallId = 1,
                StartTime = DateTime.Now.AddDays(1),
                TicketPrice = 12.50m
            };

            
            var otherTicket = new Ticket
            {
                Id = 1,
                ProjectionId = 1,
                SeatId = 1,
                UserId = otherUserId,  
                PricePaid = 12.50m,
                PurchasedAt = DateTime.Now.AddHours(-1)
            };

            _context.Movies.Add(movie);
            _context.Halls.Add(hall);
            _context.Seats.Add(seat);
            _context.Projections.Add(projection);
            _context.Tickets.Add(otherTicket);
            await _context.SaveChangesAsync();

            
            var adminUser_ = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, adminUserId),
        new Claim(ClaimTypes.Name, "admin@test.com"),
        new Claim(ClaimTypes.Role, "Admin") 
    }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = adminUser_ }
            };

            
            var result = await _controller.Details(1) as ViewResult;
            var model = result?.Model as Ticket;

           
            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Id, Is.EqualTo(1));
            Assert.That(model.UserId, Is.EqualTo(otherUserId));  

           
            Assert.That(model.Projection, Is.Not.Null);
            Assert.That(model.Projection.Movie, Is.Not.Null);
            Assert.That(model.Projection.Hall, Is.Not.Null);
            Assert.That(model.Seat, Is.Not.Null);
            Assert.That(model.User, Is.Not.Null);
            Assert.That(model.User.UserName, Is.EqualTo("other@test.com"));
        }
    }
}