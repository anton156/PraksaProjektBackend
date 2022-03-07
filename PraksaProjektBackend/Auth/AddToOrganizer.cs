using System.ComponentModel.DataAnnotations;

namespace PraksaProjektBackend.Auth
{
    public class AddToOrganizer
    {
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }
    }
}
