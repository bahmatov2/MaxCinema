namespace MaxCinema.Models
{
    public class Ticket
    {
        public int ProjectionId { get; set; }
        public Projection Projection { get; set; } = null!;

        public int SeatId { get; set; }
        public Seat Seat { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public decimal PricePaid { get; set; }
        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;


    }
}
