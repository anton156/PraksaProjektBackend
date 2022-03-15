using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PraksaProjektBackend.Auth;
using PraksaProjektBackend.Models;
using System.Security.Claims;

namespace PraksaProjektBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrentEventsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public static IWebHostEnvironment _webHostEnvironment;

        public CurrentEventsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }



        [HttpPost]
        [Route("createevent")]
        public async Task<IActionResult> Create([FromForm] CurrentEvent currentevents)
        {
            if (currentevents.EventImage.Length > 0)
            {
                string imgext = Path.GetExtension(currentevents.EventImage.FileName);
                if (imgext == ".jpg" || imgext == ".png")
                {
                    try
                    {
                        if (!Directory.Exists(_webHostEnvironment.WebRootPath + "\\ImagesEvent\\"))
                        {
                            Directory.CreateDirectory(_webHostEnvironment.WebRootPath + "\\ImagesEvent\\");
                        }

                        using (FileStream filestream = System.IO.File.Create(_webHostEnvironment.WebRootPath + "\\ImagesEvent\\" + currentevents.EventImage.FileName))
                        {
                            currentevents.EventImage.CopyTo(filestream);
                            filestream.Flush();
                            var imagename = "\\ImagesEvent\\" + currentevents.EventImage.FileName;

                            var claimsIdentity = this.User.Identity as ClaimsIdentity;
                            var userName = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                            var currentevent = new CurrentEvent
                            {
                                CurrentEventId = currentevents.CurrentEventId,
                                EventName = currentevents.EventName,
                                Content = currentevents.Content,
                                OrganizersName = userName,
                                Price = currentevents.Price,
                                NumberOfSeats = currentevents.NumberOfSeats,
                                ImagePath = imagename,
                                Begin = currentevents.Begin,
                                End = currentevents.End,
                                EventTypeId = currentevents.EventTypeId,
                                VenueId = currentevents.VenueId
                            };
                            _context.CurrentEvent.Add(currentevent);
                            var eventarh = new Event
                            {
                                EventId = currentevents.CurrentEventId,
                                EventName = currentevents.EventName,
                                Content = currentevents.Content,
                                OrganizersName = userName,
                                Begin = currentevents.Begin,
                                End = currentevents.End,
                                EventTypeId = currentevents.EventTypeId,
                                VenueId = currentevents.VenueId
                            };
                            _context.Event.Add(eventarh);
                            await _context.SaveChangesAsync();
                            return Ok(currentevent);
                        }


                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Only support .jpg and .png" });
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Creation Failed!" });
            }
        }
    }
}
