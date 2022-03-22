using PraksaProjektBackend.Filter;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        [SwaggerIgnore]
        public string? OrganizersName { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int NumberOfSeats { get; set; }

        [NotMapped]
        public IFormFile? EventImage { get; set; }
        [SwaggerIgnore]
        public string? ImagePath { get; set; }

        [Required]
        public DateTime Begin { get; set; }

        [Required]
        public DateTime End { get; set; }
        [SwaggerIgnore]
        public EventType? EventType { get; set; }

        [Required]
        public int EventTypeId { get; set; }
        [SwaggerIgnore]
        public Venue? Venue { get; set; }

        [Required]
        public int VenueId { get; set; }
        [JsonIgnore]
        public List<Ticket>? Tickets { get; set; }
    }
}
