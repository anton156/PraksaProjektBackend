namespace PraksaProjektBackend.Model
{
    public class VenueModel
    {
        public int VenueId { get; set; }

        public string? VenueName { get; set; }

        public string? Location { get; set; }

        public string? Adress { get; set; }

        public int? Capacity { get; set; }

        public bool? Booked { get; set; }


    }
}
