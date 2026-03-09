namespace MaxCinema.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public double DurationMinutes { get; set; }
        public string Description { get; set; } = string.Empty ;

        public bool IsActive { get; set; } = true;

        public string ImageUrl { get; set; } = "/images/movies/default.jpg";
        public ICollection<Projection> Projections { get; set; } = new List<Projection>();
    }
}
