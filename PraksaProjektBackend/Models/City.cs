using System.ComponentModel.DataAnnotations;

namespace PraksaProjektBackend.Models
{
    public class City
    {
        public int CityId { get; set; }

        [Required]
        public string? CityName { get; set; }

        public List<Venue>? Venues { get; set; }


    }
}
