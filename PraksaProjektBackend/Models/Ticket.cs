using System.ComponentModel.DataAnnotations;

namespace PraksaProjektBackend.Models
{
    public class Ticket
    {
        [Key]
        public int ticketId { get; set; }

        public int validUses { get; set; }

        public string? chargeId { get; set; }

        public string? userId { get; set; }

        public string? userEmail { get; set; }

        public string? qrPath { get; set; }

        public bool valid { get; set; }= true;

        public int price { get; set; }

        public DateTime start { get; set; }

        public int eventId { get; set; }

        public CurrentEvent? CurrentEvent { get; set; }

    }
}
