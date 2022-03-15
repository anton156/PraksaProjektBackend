namespace PraksaProjektBackend.Models
{
    public class Event
    {
        public int EventId { get; set; }

        public string? EventName { get; set; }

        public string? Content { get; set; }

        public string? OrganizersName { get; set; }

        public float Profit { get; set; }

        public string? EventTypeName { get; set; }
    }
}
