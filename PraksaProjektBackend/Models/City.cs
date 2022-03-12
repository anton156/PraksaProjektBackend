namespace PraksaProjektBackend.Models
{
    public class City
    {
        public int CityId { get; set; }

        public string? CityName { get; set; }

        public List<Venue>? Venues { get; set; }


    }
}
