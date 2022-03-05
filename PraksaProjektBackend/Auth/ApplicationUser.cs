using Microsoft.AspNetCore.Identity;

namespace PraksaProjektBackend.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get;  set; }
        public string? Address { get; set; }

    }
}
