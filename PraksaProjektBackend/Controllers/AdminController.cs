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
        [Route("GetAllOrganizers")]
        public async Task<IActionResult> GetAllOrganizers()
        {
            var users = _userManager.Users;
            List<User> organizers = new();
            var allOrganizers = await (from user in _dbContext.Users
                                    join userRole in _dbContext.UserRoles on user.Id
                                    equals userRole.UserId
                                    join role in _dbContext.Roles on userRole.RoleId equals role.Id
                                    where role.Name == "Organizer"
                                    select user).ToListAsync();
            foreach (var o in allOrganizers)
            {
                User organizer = new User
                {
                    Id =o.Id,
                    Firstname = o.FirstName,
                    Lastname = o.LastName,
                    Email = o.Email,
                    Address = o.Address,
                    PhoneNumber=o.PhoneNumber
                };
                organizers.Add(organizer);
            }

            if (organizers == null)
            {
                return NotFound();
            }
            return Ok(organizers);
        }

        [HttpGet]
        [Route("GetAllCustomers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var users = _userManager.Users;
            List<User> customers = new();
            var allCustomers= await (from user in _dbContext.Users
                                       join userRole in _dbContext.UserRoles on user.Id
                                       equals userRole.UserId
                                       join role in _dbContext.Roles on userRole.RoleId equals role.Id
                                       where role.Name == "Customer"
                                       select user).ToListAsync();
            foreach (var c in allCustomers)
            {
                User customer = new User
                {
                    Id = c.Id,
                    Firstname = c.FirstName,
                    Lastname = c.LastName,
                    Email = c.Email,
                    Address = c.Address,
                    PhoneNumber = c.PhoneNumber
                };
                customers.Add(customer);
            }

            if (customers == null)
            {
                return NotFound();
            }
            return Ok(customers);
        }

    }
}
