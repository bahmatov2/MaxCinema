using MaxCinema.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaxCinemaTests.ModelsTests
{
    public class MovieTests
    {
        private Movie _movie;

        [SetUp]
        public void Setup()
        {
           
            _movie = new Movie();
        }

        [Test]
        public void Movie_DefaultValues_ShouldBeSetCorrectly()
        {
            
            Assert.That(_movie.Title, Is.EqualTo(string.Empty));
            Assert.That(_movie.Genre, Is.EqualTo(string.Empty));
            Assert.That(_movie.Description, Is.EqualTo(string.Empty));
            Assert.That(_movie.DurationMinutes, Is.EqualTo(0));
            Assert.That(_movie.IsActive, Is.True); 
            Assert.That(_movie.ImageUrl, Is.EqualTo("/images/movies/default.jpg"));
            Assert.That(_movie.Projections, Is.Not.Null);
            Assert.That(_movie.Projections, Is.Empty); 
        }
        [Test]
        public void Movie_SetProperties_ShouldStoreValuesCorrectly()
        {
         
            var expectedId = 1;
            var expectedTitle = "Inception";
            var expectedGenre = "Sci-Fi";
            var expectedDescription = "A mind-bending thriller";
            var expectedDuration = 148.5;
            var expectedIsActive = false;
            var expectedImageUrl = "/images/inception.jpg";

            
            _movie.Id = expectedId;
            _movie.Title = expectedTitle;
            _movie.Genre = expectedGenre;
            _movie.Description = expectedDescription;
            _movie.DurationMinutes = expectedDuration;
            _movie.IsActive = expectedIsActive;
            _movie.ImageUrl = expectedImageUrl;

            
            Assert.Multiple(() =>
            {
                Assert.That(_movie.Id, Is.EqualTo(expectedId));
                Assert.That(_movie.Title, Is.EqualTo(expectedTitle));
                Assert.That(_movie.Genre, Is.EqualTo(expectedGenre));
                Assert.That(_movie.Description, Is.EqualTo(expectedDescription));
                Assert.That(_movie.DurationMinutes, Is.EqualTo(expectedDuration));
                Assert.That(_movie.IsActive, Is.EqualTo(expectedIsActive));
                Assert.That(_movie.ImageUrl, Is.EqualTo(expectedImageUrl));
            });
        }
        [Test]
        public void Movie_RemoveProjection_ShouldDecreaseProjectionsCount()
        {
          
            var projection1 = new Projection { Id = 1 };
            var projection2 = new Projection { Id = 2 };

            _movie.Projections.Add(projection1);
            _movie.Projections.Add(projection2);

           
            _movie.Projections.Remove(projection1);

            Assert.That(_movie.Projections.Count, Is.EqualTo(1));
            Assert.That(_movie.Projections, Does.Not.Contain(projection1));
            Assert.That(_movie.Projections, Does.Contain(projection2));
        }
        [Test]
        public void Movie_DurationMinutes_ShouldAcceptVariousValues()
        {
            
            _movie.DurationMinutes = 0;
            Assert.That(_movie.DurationMinutes, Is.EqualTo(0));

            
            _movie.DurationMinutes = 120.5;
            Assert.That(_movie.DurationMinutes, Is.EqualTo(120.5));

            
            _movie.DurationMinutes = 999.99;
            Assert.That(_movie.DurationMinutes, Is.EqualTo(999.99));
        }

    }
}

