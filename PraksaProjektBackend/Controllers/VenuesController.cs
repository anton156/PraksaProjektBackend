﻿#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using PraksaProjektBackend.Auth;
using PraksaProjektBackend.Models;

namespace PraksaProjektBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenuesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VenuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Venues
        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<Venue>>> GetVenue()
        {
            return await _context.Venue.Include(x => x.City).ToListAsync();
        }

        // GET: api/Venues/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Venue>> GetVenue(int id)
        {
            var venue = await _context.Venue.Include(x => x.City).FirstOrDefaultAsync(i => i.VenueId == id);

            if (venue == null)
            {
                return NotFound();
            }

            return venue;
        }

        // PUT: api/Venues/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVenue(int id, Venue venue)
        {
            if (id != venue.VenueId)
            {
                return BadRequest();
            }

            _context.Entry(venue).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VenueExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        [Route("disablevenue")]
        public async Task<ActionResult> DisableVenue(int id)
        {
            var venue = await _context.Venue.FindAsync(id);
            venue.Status = false;
            await _context.SaveChangesAsync();
            return Ok(venue);

            //Backup in case we run into problems with above code
            //var venue = _context.Venue.Where(x => x.VenueId == id).ToList();

            //foreach (var stat in venue)
            //{
            //    stat.Status = false;
            //}
            //_context.SaveChanges();
            //return Ok(venue);
        }

        [HttpPost]
        [Route("enablevenue")]
        public async Task<ActionResult> EnableVenue(int id)
        {
            var venue = await _context.Venue.FindAsync(id);
            venue.Status = true;
            await _context.SaveChangesAsync();
            return Ok(venue);

            //Backup in case we run into problems with above code
            //var venue = _context.Venue.Where(x => x.VenueId == id).ToList();

            //foreach (var stat in venue)
            //{
            //    stat.Status = true;
            //}
            //_context.SaveChanges();
            //return Ok(venue);
        }

        // POST: api/Venues
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Venue>> PostVenue(Venue venue)
        {
            _context.Venue.Add(venue);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVenue", new { id = venue.VenueId }, venue);
        }

        // DELETE: api/Venues/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenue(int id)
        {
            var venue = await _context.Venue.FindAsync(id);
            if (venue == null)
            {
                return NotFound();
            }

            _context.Venue.Remove(venue);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VenueExists(int id)
        {
            return _context.Venue.Any(e => e.VenueId == id);
        }
    }
}
