﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PraksaProjektBackend.Auth;
using PraksaProjektBackend.Models;
using PraksaProjektBackend.ViewModel;
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



        [HttpGet]
        [Route("getallcurrentevents")]
        public async Task<ActionResult<IEnumerable<CurrentEvent>>> GetCurrentEvent()
        {
            return await _context.CurrentEvent.Include(x => x.EventType).Include(x => x.Venue).ThenInclude(x => x.City).ToListAsync();
        }

        [HttpGet]
        [Route("getallevents")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvent()
        {
            return await _context.Event.ToListAsync();
        }

        [HttpGet]
        [Route("getcurrenteventeventbyeventtype")]
        public async Task<ActionResult<IEnumerable<CurrentEvent>>> GetCurrentEventByEventType(int id)
        {
            return await _context.CurrentEvent.Where(x=> x.EventTypeId == id).ToListAsync();
        }
        [HttpGet]
        [Route("geteventbyeventtype")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEventByEventType(int id)
        {
            return await _context.Event.Where(x => x.EventTypeId == id).ToListAsync();
        }
        [HttpGet]
        [Route("getcurrenteventbyvenue")]
        public async Task<ActionResult<IEnumerable<CurrentEvent>>> GetCurrentEventByVenue(int id)
        {
            return await _context.CurrentEvent.Where(x => x.VenueId == id).ToListAsync();
        }
        [HttpGet]
        [Route("geteventbyvenue")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEventByVenue(int id)
        {
            return await _context.Event.Where(x => x.VenueId == id).ToListAsync();
        }

        [HttpGet]
        [Route("getonecurrentevent")]
        public async Task<ActionResult<CurrentEvent>> GetCurrentEvent(int id)
        {
            var currentevent = await _context.CurrentEvent.Include(x => x.EventType).Include(x => x.Venue).ThenInclude(x => x.City).FirstOrDefaultAsync(i => i.CurrentEventId == id);

            if (currentevent == null)
            {
                return NotFound();
            }

            return currentevent;
        }

        [HttpGet]
        [Route("getoneevent")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var eventone = await _context.Event.FindAsync(id);

            if (eventone == null)
            {
                return NotFound();
            }

            return eventone;
        }

        [HttpPost]
        [Route("createevent")]
        public async Task<IActionResult> Create([FromForm] CurrentEventViewModel currentevents)
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
                            var venue = await _context.Venue.FindAsync(currentevent.VenueId);
                            if (venue.Status == false)
                            {
                                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Venue not available" });
                            }
                            if (currentevent.NumberOfSeats > venue.Capacity || currentevent.Begin > currentevent.End)
                            {
                                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Number of seat exceeds capacity or date is incorrect"});
                            }
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


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurrentEvent(int id)
        {
            var currentevent = await _context.CurrentEvent.FindAsync(id);
            if (currentevent == null)
            {
                return NotFound();
            }

            _context.CurrentEvent.Remove(currentevent);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        [Route("deletefinishedevents")]
        public async Task<IActionResult> DeleteFinishedEvents()
        {
            var currentevent = await _context.CurrentEvent.Where(x => x.End < DateTime.Now).ToListAsync();
            if (currentevent == null)
            {
                return NotFound();
            }

            _context.CurrentEvent.RemoveRange(currentevent);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPost]
        [Route("reserveticket")]
        public async Task<ActionResult> EnableVenue(int id, int reserve)
        {
            var currentevent = await _context.CurrentEvent.FindAsync(id);
            if (currentevent.NumberOfSeats < reserve)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Not enought seats" });
            }
            currentevent.NumberOfSeats = currentevent.NumberOfSeats - reserve;
            await _context.SaveChangesAsync();
            return Ok(currentevent);
        }
        }
}
