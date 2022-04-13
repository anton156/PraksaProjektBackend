using System.ComponentModel.DataAnnotations;

namespace PraksaProjektBackend.ViewModel
{
    public class PaymentViewModel
    {
        [Required]
        int EventId { get; set; }
        [Required]
        int Quantity { get; set; }
    }
}
