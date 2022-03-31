using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using PraksaProjektBackend.Auth;
using PraksaProjektBackend.Models;
using PraksaProjektBackend.Services;
using QRCoder;
using System.Drawing;


namespace PraksaProjektBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMailService _mailService;

        public AdminController(
            UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext, IMailService mailService)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _mailService = mailService;
        }



        [HttpGet]
        [Route("listallusers")]
        [EnableQuery]
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
        [HttpGet]
        [Route("sendreservedticket")]
        public async Task <dynamic> SendReservation(string eventname, string email)
        {
            return await _mailService.SendReservedQrEmailAsync(eventname, email);
        }
        [HttpGet]
        [Route("sendnewslettertoallusers")]
        public async Task<dynamic> SendNewsletterToAll(string subject, string body)
        {
            try
            {
                var users = _userManager.Users;
                foreach (var user in users)
                {
                    await _mailService.SendNewsletter(subject, body, user.Email);
                }
                return "Mails sent";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("sendmailtoallorganizers")]
        public async Task<dynamic> SendMailToOrganizers(string subject, string body)
        {
            try
            {
                var users = _userManager.GetUsersInRoleAsync("Organizer").Result;
                foreach (var user in users)
                {
                    await _mailService.SendNewsletter(subject, body, user.Email);
                }
                return "Mails sent";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("sendmailtoticketholders")]
        public async Task<dynamic> SendMailToTicketHolders(int eventid, string subject, string body)
        {
            try
            {
                var users = _dbContext.Ticket.Where(x => x.eventId == eventid).ToList();
                foreach (var user in users)
                {
                    await _mailService.SendNewsletter(subject, body, user.userEmail);
                }
                return "Mails sent";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
