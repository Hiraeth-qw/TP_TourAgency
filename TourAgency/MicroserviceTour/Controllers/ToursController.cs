using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicroserviceTour.Models;
using MicroserviceTour.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace MicroserviceTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToursController : ControllerBase
    {
        private readonly TourContext _context;

        public ToursController(TourContext context)
        {
            _context = context;
        }

        // GET: api/Tours
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TourRead>>> GetTour([FromQuery] string? location, [FromQuery] DateTime? startDate)
        {
            var query = _context.Tour.AsQueryable();

            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(t => t.Location.Contains(location));
            }
            if (startDate.HasValue)
            {
                query = query.Where(t => t.StartDate.Date >=  startDate.Value.Date);
            }
            query = query.Where(t => t.EndDate.Date >= DateTime.UtcNow.Date);
            query = query.Where(t => t.AvailableSeats > 0);

            return await query.Select(t => new TourRead
            {
                Id = t.Id,
                Title = t.Title,
                Location = t.Location,
                Description = t.Description,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Price = t.Price,
                AvailableSeats = t.AvailableSeats,
                PartnerIds = t.PartnerIds
            }).ToListAsync();
        }

        // GET: api/Tours/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TourRead>> GetTour(int id)
        {
            var tourDto = await _context.Tour
                .Where(t => t.Id == id)
                .Select(t => new TourRead
                {
                    Id = t.Id,
                    Title = t.Title,
                    Location = t.Location,
                    Description = t.Description,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    Price = t.Price,
                    AvailableSeats = t.AvailableSeats,
                    PartnerIds = t.PartnerIds
                })
                .FirstOrDefaultAsync();

            if (tourDto == null)
            {
                return NotFound();
            }

            return tourDto;
        }

        // PUT: api/Tours/5
        [HttpPut("{id}")]
        [Authorize(Roles = "ContentManager")]
        public async Task<IActionResult> PutTour(int id, TourCreateUpdate tourDto)
        {
            var tourToUpdate = await _context.Tour.FindAsync(id);

            if (tourToUpdate == null)
            {
                return NotFound($"Tour with ID {id} not found.");
            }

            // Mapping DTO на сущность
            tourToUpdate.Title = tourDto.Title;
            tourToUpdate.Location = tourDto.Location;
            tourToUpdate.Description = tourDto.Description;
            tourToUpdate.StartDate = tourDto.StartDate;
            tourToUpdate.EndDate = tourDto.EndDate;
            tourToUpdate.Price = tourDto.Price;
            tourToUpdate.AvailableSeats = tourDto.AvailableSeats;
            tourToUpdate.PartnerIds = tourDto.PartnerIds;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TourExists(id))
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

        // POST: api/Tours
        [HttpPost]
        [Authorize(Roles = "ContentManager")]
        public async Task<ActionResult<Tour>> PostTour(TourCreateUpdate tourDto)
        {
            // Mapping DTO на Tour
            var newTour = new Tour
            {
                Title = tourDto.Title,
                Location = tourDto.Location,
                Description = tourDto.Description,
                StartDate = tourDto.StartDate,
                EndDate = tourDto.EndDate,
                Price = tourDto.Price,
                AvailableSeats = tourDto.AvailableSeats,
                PartnerIds = tourDto.PartnerIds
            };

            _context.Tour.Add(newTour);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTour", new { id = newTour.Id }, newTour);
        }

        // DELETE: api/Tours/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "ContentManager")]
        public async Task<IActionResult> DeleteTour(int id)
        {
            var tour = await _context.Tour.FindAsync(id);
            if (tour == null)
            {
                return NotFound();
            }

            _context.Tour.Remove(tour);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/Tours/5/reserve-seat
        [HttpPatch("{id}/reserve-seat")]
        [Authorize]
        public async Task<IActionResult> ReserveSeat(int id)
        {
            var tour = await _context.Tour.FindAsync(id);

            if (tour == null) return NotFound();

            if (tour.AvailableSeats > 0)
            {
                tour.AvailableSeats--;
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return Conflict("No available seats.");
            }
        }

        private bool TourExists(int id)
        {
            return _context.Tour.Any(e => e.Id == id);
        }
    }
}
