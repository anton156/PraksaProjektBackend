namespace PraksaProjektBackend.Models
{
    public class Ticket
    {
        public int ticketId { get; set; }

        public int validUses { get; set; }

        public string? chargeId { get; set; }

        public int eventId { get; set; }

    }
}
