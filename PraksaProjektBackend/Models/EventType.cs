using PraksaProjektBackend.Filter;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PraksaProjektBackend.Models
{
    public class EventType
    {
        [Key]
        [SwaggerIgnore]
        public int EventTypeId { get; set; }

        [Required]
        public string? EventTypeName { get; set; }
        [JsonIgnore]
        public List<CurrentEvent>? CurrentEvents { get; set; }
    }
}
