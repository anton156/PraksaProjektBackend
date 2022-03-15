using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PraksaProjektBackend.Auth;
using PraksaProjektBackend.Models;

namespace PraksaProjektBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public AdminController(
            UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }



        [HttpGet]
        [Route("listallusers")]
        public IActionResult ListUsers()
        {
            var users = _userManager.Users;
            return Ok(users);
        }
        [HttpGet]
        [Route("listorganizers")]
        public IActionResult ListOrganizer()
        {
            var users = _userManager.GetUsersInRoleAsync("Organizer").Result;
            return Ok(users);
        }
        [HttpGet]
        [Route("listcustomers")]
        public IActionResult ListCustomer()
        {
            var users = _userManager.GetUsersInRoleAsync("Customer").Result;
            return Ok(users);
        }
        

    }
}
