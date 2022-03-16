using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PraksaProjektBackend.Models
{
    public class City
    {
        public int CityId { get; set; }

        [Required]
        public string? CityName { get; set; }
        [JsonIgnore]
        public List<Venue>? Venues { get; set; }


    }
}
