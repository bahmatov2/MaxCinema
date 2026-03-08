namespace MaxCinema.Models
{
    public class Hall
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int TotalRows { get; set; }
        public int SeatsPerRow { get; set; }

        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
        public ICollection<Projection> Projections { get; set; } = new List<Projection>();

    }
}
