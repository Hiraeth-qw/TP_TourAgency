using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicroservicePartner.Models;
using MicroservicePartner.DTOs;

namespace MicroservicePartner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnersController : ControllerBase
    {
        private readonly PartnerContext _context;

        public PartnersController(PartnerContext context)
        {
            _context = context;
        }

        // GET: api/Partners
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Partner>>> GetPartners()
        {
            return await _context.Partners.ToListAsync();
        }

        // GET: api/Partners/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Partner>> GetPartner(long id)
        {
            var partner = await _context.Partners.FindAsync(id);

            if (partner == null)
            {
                return NotFound();
            }

            return partner;
        }

        // PUT: api/Partners/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPartner(long id, Partner partner)
        {
            if (id != partner.Id)
            {
                return BadRequest();
            }

            _context.Entry(partner).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PartnerExists(id))
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

        // POST: api/Partners/hotel
        [HttpPost("hotel")]
        public async Task<ActionResult<partnerHotel>> PostHotel(partnerHotel hotel)
        {
            _context.Partners.Add(hotel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHotel), new { id = hotel.Id }, hotel);
        }

        // POST: api/Partners/operator
        [HttpPost("operator")]
        public async Task<ActionResult<partnerOperator>> PostOperator(partnerOperator @operator)
        {
            _context.Partners.Add(@operator);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOperator), new { id = @operator.Id }, @operator);
        }

        // POST: api/Partners/transport
        [HttpPost("transport")]
        public async Task<ActionResult<partnerTransport>> PostTransportPartner(partnerTransport transportPartner)
        {
            _context.Partners.Add(transportPartner);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTransportPartner), new { id = transportPartner.Id }, transportPartner);
        }

        // DELETE: api/Partners/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePartner(long id)
        {
            var partner = await _context.Partners.FindAsync(id);
            if (partner == null)
            {
                return NotFound();
            }

            _context.Partners.Remove(partner);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/partners/hotels
        [HttpGet("hotels")]
        public async Task<ActionResult<IEnumerable<partnerHotel>>> GetHotels()
        {
            var hotels = await _context.Partners.OfType<partnerHotel>().ToListAsync();

            if (hotels == null || !hotels.Any())
            {
                return NotFound("No hotels found.");
            }

            return Ok(hotels);
        }
        
        // GET: api/partners/hotels/{id}
        [HttpGet("hotels/{id}")]
        public async Task<ActionResult<partnerHotel>> GetHotel(long id)
        {
            var hotel = await _context.Partners.OfType<partnerHotel>().FirstOrDefaultAsync(h => h.Id == id);

            if (hotel == null)
            {
                return NotFound($"Hotel with ID {id} not found.");
            }

            return Ok(hotel);
        }

        // GET: api/Partners/operators
        [HttpGet("operators")]
        public async Task<ActionResult<IEnumerable<partnerOperator>>> GetOperators()
        {
            var operators = await _context.Partners.OfType<partnerOperator>().ToListAsync();

            if (operators == null || !operators.Any())
            {
                return NotFound("No operators found.");
            }

            return Ok(operators);
        }

        // GET: api/Partners/operators/{id}
        [HttpGet("operators/{id}")]
        public async Task<ActionResult<partnerOperator>> GetOperator(long id)
        {
            var @operator = await _context.Partners.OfType<partnerOperator>().FirstOrDefaultAsync(o => o.Id == id);

            if (@operator == null)
            {
                return NotFound($"Operator with ID {id} not found.");
            }

            return Ok(@operator);
        }

        // GET: api/Partners/transport
        [HttpGet("transport")]
        public async Task<ActionResult<IEnumerable<partnerTransport>>> GetTransportPartners()
        {
            var transports = await _context.Partners.OfType<partnerTransport>().ToListAsync();

            if (transports == null || !transports.Any())
            {
                return NotFound("No transport partners found.");
            }

            return Ok(transports);
        }

        // GET: api/Partners/transport/{id}
        [HttpGet("transport/{id}")]
        public async Task<ActionResult<partnerTransport>> GetTransportPartner(long id)
        {
            var transport = await _context.Partners.OfType<partnerTransport>().FirstOrDefaultAsync(t => t.Id == id);

            if (transport == null)
            {
                return NotFound($"Transport partner with ID {id} not found.");
            }

            return Ok(transport);
        }

        [HttpPost("confirm-booking")]
        public async Task<ActionResult<bool>> ConfirmBooking([FromBody] PartnerConfirmationRequest request)
        {
            var partner = await _context.Partners.FindAsync(request.PartnerId);
            if (partner == null)
            {
                return NotFound($"Partner with ID {request.PartnerId} not found in the system.");
            }

            bool isServiceAvailable = true;

            if (request.ServiceStartDate.Date == DateTime.Now.Date)
            {
                isServiceAvailable = false;
            }
            else if (partner.PartnerType == Models.Type.Hotel && request.ServiceStartDate.Date <= DateTime.Now.AddDays(1).Date)
            {
                isServiceAvailable = false;
            }

            if (isServiceAvailable)
            {

                return Ok(true);
            }
            else
            {

                return Ok(false);
            }
        }

        private bool PartnerExists(long id)
        {
            return _context.Partners.Any(e => e.Id == id);
        }
    }
}
