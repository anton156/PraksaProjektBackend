using PraksaProjektBackend.Filter;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PraksaProjektBackend.Models
{
    public class Venue
    {
        [Key]
        [SwaggerIgnore]
        public int VenueId { get; set; }

        [Required]
        public string? VenueName { get; set; }

        [Required]
        public string? Address { get; set; }

        [Required]
        public int? Capacity { get; set; }

        public bool? Status { get; set; } = true;
        [SwaggerIgnore]
        public City? City { get; set; }

        [Required]
        public int CityId { get; set; }
        [JsonIgnore]
        public List<CurrentEvent>? CurrentEvents { get; set; }
    }
}
