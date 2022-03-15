namespace PraksaProjektBackend.Models
{
    public class EventType
    {
        public int EventTypeId { get; set; }

        public string? EventTypeName { get; set; }

        public List<CurrentEvent>? CurrentEvents { get; set; }
    }
}
