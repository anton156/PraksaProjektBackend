using PraksaProjektBackend.Filter;
using System.ComponentModel.DataAnnotations;

namespace PraksaProjektBackend.ViewModel
{
    public class CurrentEventViewModel
    {

        
        [SwaggerIgnore]
        public int CurrentEventId { get; set; }

        [Required]
        public string? EventName { get; set; }

        [Required]
        public string? Content { get; set; }
        [Required]
        public float Price { get; set; }

        [Required]
        public int NumberOfSeats { get; set; }

        public IFormFile? EventImage { get; set; }

        [Required]
        public DateTime Begin { get; set; }

        [Required]
        public DateTime End { get; set; }
        [Required]
        public int EventTypeId { get; set; }
        [Required]
        public int VenueId { get; set; }
    }
}
