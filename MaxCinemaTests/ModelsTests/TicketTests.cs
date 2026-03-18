using MaxCinema.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaxCinemaTests.ModelsTests
{
    public class TicketTests
    {
        private Ticket _ticket;
        [SetUp]
        public void Setup()
        {
            _ticket = new Ticket();
        }

        [Test]
        public void Ticket_DefaultValues_ShouldBeSetCorrectly()
        {
            Assert.That(_ticket.Id, Is.EqualTo(0));
            Assert.That(_ticket.UserId, Is.EqualTo(string.Empty));
            Assert.That(_ticket.PricePaid, Is.EqualTo(0));
            Assert.That(_ticket.PurchasedAt, Is.Not.EqualTo(default(DateTime)));
        }
        [Test]
        public void Ticket_SetProperties_ShouldStoreValuesCorrectly()
        {
            var expectedId = 1;
            var expectedProjectionId = 2;
            var expectedSeatId = 3;
            var expectedUserId = "user-123";
            var expectedPrice = 12.50m;
            var expectedDate = new DateTime(2026, 3, 17);

           
            _ticket.Id = expectedId;
            _ticket.ProjectionId = expectedProjectionId;
            _ticket.SeatId = expectedSeatId;
            _ticket.UserId = expectedUserId;
            _ticket.PricePaid = expectedPrice;
            _ticket.PurchasedAt = expectedDate;

            
            Assert.Multiple(() =>
            {
                Assert.That(_ticket.Id, Is.EqualTo(expectedId));
                Assert.That(_ticket.ProjectionId, Is.EqualTo(expectedProjectionId));
                Assert.That(_ticket.SeatId, Is.EqualTo(expectedSeatId));
                Assert.That(_ticket.UserId, Is.EqualTo(expectedUserId));
                Assert.That(_ticket.PricePaid, Is.EqualTo(expectedPrice));
                Assert.That(_ticket.PurchasedAt, Is.EqualTo(expectedDate));
            });
        }
        [Test]
        public void Ticket_PricePaid_ShouldAcceptDecimalValues()
        {
            
            _ticket.PricePaid = 0m;
            Assert.That(_ticket.PricePaid, Is.EqualTo(0m));

            
            _ticket.PricePaid = 9.99m;
            Assert.That(_ticket.PricePaid, Is.EqualTo(9.99m));

            
            _ticket.PricePaid = 999.99m;
            Assert.That(_ticket.PricePaid, Is.EqualTo(999.99m));
        }
        [Test]
        public void Ticket_NavigationProperties_ShouldBeAssignable()
        {
            
            var projection = new Projection { Id = 1 };
            var seat = new Seat { Id = 1, Row = 3, Number = 5 };
            var user = new ApplicationUser { FirstName = "Иван" };

            _ticket.Projection = projection;
            _ticket.Seat = seat;
            _ticket.User = user;

            Assert.That(_ticket.Projection, Is.Not.Null);
            Assert.That(_ticket.Projection.Id, Is.EqualTo(1));
            Assert.That(_ticket.Seat.Row, Is.EqualTo(3));
            Assert.That(_ticket.Seat.Number, Is.EqualTo(5));
            Assert.That(_ticket.User.FirstName, Is.EqualTo("Иван"));
        }

        [Test]
        public void Ticket_PurchasedAt_ShouldDefaultToCurrentTime()
        {
          
            var before = DateTime.UtcNow.AddSeconds(-1);
            var newTicket = new Ticket();
            var after = DateTime.UtcNow.AddSeconds(1);

            Assert.That(newTicket.PurchasedAt, Is.InRange(before, after));
        }
    }
}
