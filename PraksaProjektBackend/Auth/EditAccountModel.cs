using System.ComponentModel.DataAnnotations;

namespace PraksaProjektBackend.Auth
{
    public class EditAccountModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string? Firstname { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string? Lastname { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Adress is required")]
        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
