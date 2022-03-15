using System.ComponentModel.DataAnnotations;

namespace PraksaProjektBackend.Models
{
    public class Venue
    {
        [Key]
        public int VenueId { get; set; }

        public string? VenueName { get; set; }

        public string? Address { get; set; }

        public int? Capacity { get; set; }

        public bool? Status { get; set; }

        public City? City { get; set; }

        public int CityId { get; set; }

        public List<CurrentEvent>? CurrentEvents { get; set; }
    }
}
