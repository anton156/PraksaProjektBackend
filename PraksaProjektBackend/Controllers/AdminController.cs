using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PraksaProjektBackend.Auth;
using PraksaProjektBackend.Models;
using QRCoder;
using System.Drawing;


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
        [ApiExplorerSettings(IgnoreApi = true)]
        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }
        [HttpGet]
        [Route("sendreservedticket")]
        public async Task <IActionResult> SendReservation(string eventname)
        {
            QRCodeGenerator _qrCode = new QRCodeGenerator();
            QRCodeData _qrCodeData = _qrCode.CreateQrCode("Reserved Ticket for " + eventname, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode (_qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.DarkRed, Color.PaleGreen, true);
            var bytes = ImageToByteArray(qrCodeImage);
            return File(bytes, "image/bmp");
        }

    }
}
