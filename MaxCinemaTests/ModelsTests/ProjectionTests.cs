using MaxCinema.Models;

namespace MaxCinemaTests.ModelsTests
{
    public class ProjectionTests
    {
        private Projection _projection;

        [SetUp]
        public void Setup()
        {
            _projection = new Projection();
        }

        [Test]
        public void Projection_DefaultValues_ShouldBeSetCorrectly()
        {
            Assert.That(_projection.Id, Is.EqualTo(0));
            Assert.That(_projection.MovieId, Is.EqualTo(0));
            Assert.That(_projection.HallId, Is.EqualTo(0));
            Assert.That(_projection.TicketPrice, Is.EqualTo(0));
            Assert.That(_projection.Tickets, Is.Not.Null);
            Assert.That(_projection.Tickets, Is.Empty);
        }

        [Test]
        public void Projection_SetProperties_ShouldStoreValuesCorrectly()
        {
            
            var expectedId = 1;
            var expectedMovieId = 2;
            var expectedHallId = 3;
            var expectedStartTime = new DateTime(2026, 5, 1, 19, 30, 0);
            var expectedPrice = 12.50m;

           
            _projection.Id = expectedId;
            _projection.MovieId = expectedMovieId;
            _projection.HallId = expectedHallId;
            _projection.StartTime = expectedStartTime;
            _projection.TicketPrice = expectedPrice;

           
            Assert.Multiple(() =>
            {
                Assert.That(_projection.Id, Is.EqualTo(expectedId));
                Assert.That(_projection.MovieId, Is.EqualTo(expectedMovieId));
                Assert.That(_projection.HallId, Is.EqualTo(expectedHallId));
                Assert.That(_projection.StartTime, Is.EqualTo(expectedStartTime));
                Assert.That(_projection.TicketPrice, Is.EqualTo(expectedPrice));
            });
        }

        [Test]
        public void Projection_NavigationProperties_ShouldBeAssignable()
        {
           
            var movie = new Movie { Id = 1, Title = "Inception", Genre = "Sci-Fi", DurationMinutes = 148 };
            var hall = new Hall { Id = 1, Name = "Зала 1", TotalRows = 8, SeatsPerRow = 10 };

           
            _projection.Movie = movie;
            _projection.Hall = hall;

           
            Assert.That(_projection.Movie, Is.Not.Null);
            Assert.That(_projection.Movie.Title, Is.EqualTo("Inception"));
            Assert.That(_projection.Hall, Is.Not.Null);
            Assert.That(_projection.Hall.Name, Is.EqualTo("Зала 1"));
        }

        [Test]
        public void Projection_AddTicket_ShouldIncreaseTicketsCount()
        {
            
            var ticket = new Ticket { Id = 1, PricePaid = 12.50m };

            
            _projection.Tickets!.Add(ticket);

            
            Assert.That(_projection.Tickets.Count, Is.EqualTo(1));
            Assert.That(_projection.Tickets, Does.Contain(ticket));
        }

        [Test]
        public void Projection_RemoveTicket_ShouldDecreaseTicketsCount()
        {
           
            var ticket1 = new Ticket { Id = 1, PricePaid = 12.50m };
            var ticket2 = new Ticket { Id = 2, PricePaid = 9.99m };
            _projection.Tickets!.Add(ticket1);
            _projection.Tickets!.Add(ticket2);

           
            _projection.Tickets.Remove(ticket1);

         
            Assert.That(_projection.Tickets.Count, Is.EqualTo(1));
            Assert.That(_projection.Tickets, Does.Not.Contain(ticket1));
            Assert.That(_projection.Tickets, Does.Contain(ticket2));
        }

        [Test]
        public void Projection_TicketPrice_ShouldAcceptDecimalValues()
        {
            _projection.TicketPrice = 0m;
            Assert.That(_projection.TicketPrice, Is.EqualTo(0m));

            _projection.TicketPrice = 9.99m;
            Assert.That(_projection.TicketPrice, Is.EqualTo(9.99m));

            _projection.TicketPrice = 99.99m;
            Assert.That(_projection.TicketPrice, Is.EqualTo(99.99m));
        }

        [Test]
        public void Projection_StartTime_ShouldBeInFuture()
        {
           
            var futureTime = DateTime.Now.AddDays(7);

            
            _projection.StartTime = futureTime;

           
            Assert.That(_projection.StartTime, Is.GreaterThan(DateTime.Now));
        }
    }
}