using System.ComponentModel.DataAnnotations;

namespace PraksaProjektBackend.Models
{
    public class EventType
    {
        [Key]
        public int EventTypeId { get; set; }

        [Required]
        public string? EventTypeName { get; set; }

        public List<CurrentEvent>? CurrentEvents { get; set; }
    }
}
