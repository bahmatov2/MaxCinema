using MaxCinema.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaxCinemaTests.ModelsTests
{
    public class HallTests
    {
        private Hall _hall;

        [SetUp]
        public void Setup()
        {
            _hall = new Hall();
        }

        [Test]
        public void Hall_DefaultValues_ShouldBeSetCorrectly()
        {
         
            Assert.That(_hall.Id, Is.EqualTo(0));
            Assert.That(_hall.Name, Is.EqualTo(string.Empty));
            Assert.That(_hall.TotalRows, Is.EqualTo(0));
            Assert.That(_hall.SeatsPerRow, Is.EqualTo(0));
            Assert.That(_hall.Seats, Is.Not.Null);
            Assert.That(_hall.Seats, Is.Empty);
            Assert.That(_hall.Projections, Is.Not.Null);
            Assert.That(_hall.Projections, Is.Empty);
        }

        [Test]
        public void Hall_SetProperties_ShouldStoreValuesCorrectly()
        {
            
            var expectedId = 1;
            var expectedName = "Зала 1";
            var expectedRows = 8;
            var expectedSeatsPerRow = 10;

            _hall.Id = expectedId;
            _hall.Name = expectedName;
            _hall.TotalRows = expectedRows;
            _hall.SeatsPerRow = expectedSeatsPerRow;

            
            Assert.Multiple(() =>
            {
                Assert.That(_hall.Id, Is.EqualTo(expectedId));
                Assert.That(_hall.Name, Is.EqualTo(expectedName));
                Assert.That(_hall.TotalRows, Is.EqualTo(expectedRows));
                Assert.That(_hall.SeatsPerRow, Is.EqualTo(expectedSeatsPerRow));
            });
        }

        [Test]
        public void Hall_TotalCapacity_ShouldBeCalculatedCorrectly()
        {
          
            _hall.TotalRows = 8;
            _hall.SeatsPerRow = 10;

            
            var totalCapacity = _hall.TotalRows * _hall.SeatsPerRow;

           
            Assert.That(totalCapacity, Is.EqualTo(80));
        }

        [Test]
        public void Hall_AddSeat_ShouldIncreaseSeatsCount()
        {
            
            var seat = new Seat { Id = 1, Row = 1, Number = 1 };

           
            _hall.Seats.Add(seat);

           
            Assert.That(_hall.Seats.Count, Is.EqualTo(1));
            Assert.That(_hall.Seats, Does.Contain(seat));
        }

        [Test]
        public void Hall_RemoveSeat_ShouldDecreaseSeatsCount()
        {
           
            var seat1 = new Seat { Id = 1, Row = 1, Number = 1 };
            var seat2 = new Seat { Id = 2, Row = 1, Number = 2 };
            _hall.Seats.Add(seat1);
            _hall.Seats.Add(seat2);

           
            _hall.Seats.Remove(seat1);

           
            Assert.That(_hall.Seats.Count, Is.EqualTo(1));
            Assert.That(_hall.Seats, Does.Not.Contain(seat1));
            Assert.That(_hall.Seats, Does.Contain(seat2));
        }

        [Test]
        public void Hall_AddProjection_ShouldIncreaseProjectionsCount()
        {
            
            var projection = new Projection { Id = 1 };

            
            _hall.Projections.Add(projection);

           
            Assert.That(_hall.Projections.Count, Is.EqualTo(1));
            Assert.That(_hall.Projections, Does.Contain(projection));
        }

        [Test]
        public void Hall_MultipleSeats_ShouldAllBeStored()
        {
            
            for (int i = 1; i <= 5; i++)
            {
                _hall.Seats.Add(new Seat { Id = i, Row = 1, Number = i });
            }


            Assert.That(_hall.Seats.Count, Is.EqualTo(5));
        }
    }
}
