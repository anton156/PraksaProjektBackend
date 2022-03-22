using PraksaProjektBackend.Filter;

namespace PraksaProjektBackend.Models
{
    public class Payment
    {
        public string cardnumber { get; set; }

        public int month { get; set; }

        public int year { get; set; }

        public string cvc { get; set; }
        [SwaggerIgnore]
        public int value { get; set; }
    }
}
