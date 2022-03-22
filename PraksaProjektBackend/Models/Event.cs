using System.ComponentModel.DataAnnotations;

namespace PraksaProjektBackend.Models
{
    public class Event
    {
        public int EventId { get; set; }

        public string? EventName { get; set; }

        public DateTime? Begin { get; set; }

        public DateTime? End { get; set; }

        public string? Content { get; set; }

        public string? OrganizersName { get; set; }

        public int Profit { get; set; } = 0;

        public int? EventTypeId { get; set; }

        public int? VenueId { get; set; }
    }
}
