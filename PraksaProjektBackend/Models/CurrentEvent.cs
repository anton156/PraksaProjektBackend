using System.ComponentModel.DataAnnotations.Schema;

namespace PraksaProjektBackend.Models
{
    public class CurrentEvent
    {
        public int CurrentEventId { get; set; }

        public string? EventName { get; set; }

        public string? Content { get; set; }

        public string? OrganizersName { get; set; }

        public float Price { get; set; }

        public int NumberOfSeats { get; set; }

        [NotMapped]
        public IFormFile? EventImage { get; set; }

        public string? ImagePath { get; set; }

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        public EventType? EventType { get; set; }

        public int EventTypeId { get; set; }

        public Venue? Venue { get; set; }

        public int VenueId { get; set; }
    }
}
