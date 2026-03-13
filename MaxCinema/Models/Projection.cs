using System.Net.Sockets;

namespace MaxCinema.Models
{
    public class Projection
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
        public int HallId { get; set; }
        public Hall Hall { get; set; } = null!;

        public DateTime StartTime { get; set; }
        public decimal TicketPrice { get; set; }

        public ICollection<Ticket>? Tickets { get; set; } = new List<Ticket>();
    }
}
