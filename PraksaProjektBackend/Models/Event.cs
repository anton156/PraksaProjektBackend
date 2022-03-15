using System.ComponentModel.DataAnnotations;

namespace PraksaProjektBackend.Models
{
    public class Event
    {
        public int EventId { get; set; }

        public string? EventName { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [DataType(DataType.Date)]
        public DateOnly? EventDate { get; set; }

        public string? Content { get; set; }

        public string? OrganizersName { get; set; }

        public float Profit { get; set; }

        public string? EventTypeName { get; set; }
    }
}
