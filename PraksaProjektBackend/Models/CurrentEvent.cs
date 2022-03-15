using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PraksaProjektBackend.Models
{
    public class CurrentEvent
    {
        [Key]
        public int CurrentEventId { get; set; }
        
        [Required]
        public string? EventName { get; set; }

        [Required]
        public string? Content { get; set; }

        public string? OrganizersName { get; set; }

        [Required]
        public float Price { get; set; }

        [Required]
        public int NumberOfSeats { get; set; }

        [NotMapped]
        public IFormFile? EventImage { get; set; }

        public string? ImagePath { get; set; }

        [Required]
        public DateTime Begin { get; set; }

        [Required]
        public DateTime End { get; set; }

        public EventType? EventType { get; set; }

        [Required]
        public int EventTypeId { get; set; }

        public Venue? Venue { get; set; }

        [Required]
        public int VenueId { get; set; }
    }
}
