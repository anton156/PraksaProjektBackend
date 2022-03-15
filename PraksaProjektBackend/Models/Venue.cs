using System.ComponentModel.DataAnnotations;

namespace PraksaProjektBackend.Models
{
    public class Venue
    {
        [Key]
        public int VenueId { get; set; }

        [Required]
        public string? VenueName { get; set; }

        [Required]
        public string? Address { get; set; }

        [Required]
        public int? Capacity { get; set; }

        public bool? Status { get; set; } = true;

        public City? City { get; set; }

        [Required]
        public int CityId { get; set; }

        public List<CurrentEvent>? CurrentEvents { get; set; }
    }
}
