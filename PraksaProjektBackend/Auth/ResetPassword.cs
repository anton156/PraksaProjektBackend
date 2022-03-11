using System.ComponentModel.DataAnnotations;

namespace PraksaProjektBackend.Auth
{
    public class ResetPassword
    {
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "ConfirmPassword is required")]
        [Compare ("Password", ErrorMessage ="Password and Confirm password must match")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Token is required")]
        public string? Token { get; set; }
    }
}
