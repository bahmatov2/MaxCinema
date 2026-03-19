using MaxCinema.Models;

namespace MaxCinemaTests.ModelsTests
{
    public class SeatTests
    {
        private Seat _seat;

        [SetUp]
        public void Setup()
        {
            _seat = new Seat();
        }

        [Test]
        public void Seat_DefaultValues_ShouldBeSetCorrectly()
        {
            Assert.That(_seat.Id, Is.EqualTo(0));
            Assert.That(_seat.HallId, Is.EqualTo(0));
            Assert.That(_seat.Row, Is.EqualTo(0));
            Assert.That(_seat.Number, Is.EqualTo(0));
            Assert.That(_seat.Tickets, Is.Not.Null);
            Assert.That(_seat.Tickets, Is.Empty);
        }

        [Test]
        public void Seat_SetProperties_ShouldStoreValuesCorrectly()
        {
          
            var expectedId = 1;
            var expectedHallId = 2;
            var expectedRow = 3;
            var expectedNumber = 5;

            
            _seat.Id = expectedId;
            _seat.HallId = expectedHallId;
            _seat.Row = expectedRow;
            _seat.Number = expectedNumber;

           
            Assert.Multiple(() =>
            {
                Assert.That(_seat.Id, Is.EqualTo(expectedId));
                Assert.That(_seat.HallId, Is.EqualTo(expectedHallId));
                Assert.That(_seat.Row, Is.EqualTo(expectedRow));
                Assert.That(_seat.Number, Is.EqualTo(expectedNumber));
            });
        }

        [Test]
        public void Seat_NavigationProperty_HallShouldBeAssignable()
        {
            
            var hall = new Hall { Id = 1, Name = "Зала 1", TotalRows = 8, SeatsPerRow = 10 };

          
            _seat.Hall = hall;

           
            Assert.That(_seat.Hall, Is.Not.Null);
            Assert.That(_seat.Hall.Name, Is.EqualTo("Зала 1"));
        }

        [Test]
        public void Seat_AddTicket_ShouldIncreaseTicketsCount()
        {
            
            var ticket = new Ticket { Id = 1, PricePaid = 12.50m };

            
            _seat.Tickets.Add(ticket);

            
            Assert.That(_seat.Tickets.Count, Is.EqualTo(1));
            Assert.That(_seat.Tickets, Does.Contain(ticket));
        }

        [Test]
        public void Seat_RemoveTicket_ShouldDecreaseTicketsCount()
        {
           
            var ticket1 = new Ticket { Id = 1, PricePaid = 12.50m };
            var ticket2 = new Ticket { Id = 2, PricePaid = 9.99m };
            _seat.Tickets.Add(ticket1);
            _seat.Tickets.Add(ticket2);

           
            _seat.Tickets.Remove(ticket1);

            
            Assert.That(_seat.Tickets.Count, Is.EqualTo(1));
            Assert.That(_seat.Tickets, Does.Not.Contain(ticket1));
            Assert.That(_seat.Tickets, Does.Contain(ticket2));
        }

        [Test]
        public void Seat_RowAndNumber_ShouldIdentifySeatUniquely()
        {
            
            var seat1 = new Seat { Id = 1, Row = 3, Number = 5 };
            var seat2 = new Seat { Id = 2, Row = 3, Number = 6 };

            
            Assert.That(seat1.Number, Is.Not.EqualTo(seat2.Number));
            Assert.That(seat1.Row, Is.EqualTo(seat2.Row));
        }

        [Test]
        public void Seat_MultipleTickets_ShouldAllBeStored()
        {
           
            for (int i = 1; i <= 5; i++)
            {
                _seat.Tickets.Add(new Ticket { Id = i, PricePaid = 9.99m });
            }
            Assert.That(_seat.Tickets.Count, Is.EqualTo(5));
        }
    }
}