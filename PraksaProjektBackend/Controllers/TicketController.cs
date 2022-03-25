using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PraksaProjektBackend.Auth;
using PraksaProjektBackend.Models;
using System.Security.Claims;

namespace PraksaProjektBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public static IWebHostEnvironment? _webHostEnvironment;

        public TicketController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [Route("getusersticket")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetUserTickets()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.Hash)?.Value;
            return await _context.Ticket.Where(x => x.userId == userId).ToListAsync();
        }
        [HttpGet]
        [Route("getticketqrcode")]
        public async Task<ActionResult<Ticket>> GetTicketQrCode(int id)
        {
            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
            {
                return NotFound();
            }
            var userId = claimsIdentity.FindFirst(ClaimTypes.Hash)?.Value;
            if(userId != ticket.userId)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Not your ticket bucko" });
            }
            var path = _webHostEnvironment.WebRootPath + "\\QRcode\\QR_" + ticket.chargeId + ".jpg";
            Byte[] b = System.IO.File.ReadAllBytes(path);        
            return File(b, "image/jpeg");
        }
    }
}
