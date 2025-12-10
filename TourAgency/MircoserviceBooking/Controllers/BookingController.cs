using System.Security.Claims;
using Azure.Core;
using MicroserviceBooking.DTOs;
using MicroserviceBooking.Models;
using MicroserviceBooking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MicroserviceBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly BookingContext _context;
        private readonly TourService _tourService;
        private readonly PartnerService _partnerService;
        private readonly PaymentService _paymentService;

        public BookingController(BookingContext context, TourService tourService, PartnerService partnerService, PaymentService paymentService)
        {
            _context = context;
            _tourService = tourService;
            _partnerService = partnerService;
            _paymentService = paymentService;
        }

        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
        private string? GetToken() => Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        // GET: api/booking/plan
        [HttpGet("plan")]
        public async Task<IActionResult> GetMyPlan()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.AddedDate)
                .ToListAsync();

            var result = new List<CartItemDto>();

            foreach (var item in cartItems)
            {
                var tour = await _tourService.GetTourAsync(item.TourId);

                if (tour != null)
                {
                    result.Add(new CartItemDto
                    {
                        CartItemId = item.Id,
                        TourId = item.TourId,
                        Title = tour.Title,
                        Location = tour.Location,
                        Price = tour.Price,
                        StartDate = tour.StartDate,
                        NumberOfSeats = item.NumberOfSeats,
                        TotalPrice = tour.Price * item.NumberOfSeats,
                        AddedDate = item.AddedDate
                    });
                }
            }

            return Ok(result);
        }

        // POST: api/booking/plan/add
        [HttpPost("plan/add")]
        public async Task<IActionResult> AddToPlan([FromBody] AddItemToCartDto request)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var tour = await _tourService.GetTourAsync(request.TourId);
            if (tour == null) return NotFound("Tour not found.");

            var cartItem = new CartItem
            {
                UserId = userId,    
                TourId = request.TourId,
                NumberOfSeats = request.TouristsNumber
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Added to cart", ItemId = cartItem.Id });
        }

        // DELETE: api/booking/plan/{id}
        [HttpDelete("plan/{id}")]
        public async Task<IActionResult> RemoveFromPlan(int id)
        {
            var userId = GetUserId();
            var item = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == id);

            if (item == null) return NotFound();
            if (item.UserId != userId) return Forbid();

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/booking/
        [HttpPost]
        [Authorize(Roles = "Client, Manager, Admin")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
        {
            var currentUserId = GetUserId();
            var token = GetToken();
            if (currentUserId == null || token == null) return Unauthorized();

            string targetUserId = currentUserId;

            bool isStaff = User.IsInRole("Admin") || User.IsInRole("Manager");
            if (isStaff && !string.IsNullOrEmpty(request.ClientUserId)) 
                targetUserId = request.ClientUserId;

            // Информация о туре
            var tour = await _tourService.GetTourAsync(request.TourId);
            if (tour == null) return NotFound("Tour not found.");
            if (tour.AvailableSeats < request.TouristsNumber)
                return Conflict("Not enough seats available.");

            var booking = new Booking
            {
                UserId = targetUserId,
                TourId = request.TourId,
                NumberOfSeats = request.TouristsNumber,
                TotalAmount = tour.Price * request.TouristsNumber,
                Status = BookingStatus.PendingPartners
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Опрос партнёров
            bool allPartnersConfirmed = true;

            foreach (var partnerId in tour.PartnerIds)
            {
                var confirmReq = new PartnerConfirmationRequest
                {
                    PartnerId = partnerId,
                    BookingId = booking.Id,
                    TourId = tour.Id,
                    ServiceStartDate = tour.StartDate,
                    Details = $"Booking for {request.TouristsNumber} persons"
                };

                bool isConfirmed = await _partnerService.ConfirmBookingAsync(confirmReq, token);

                var singleConf = new SingleConfirmation
                {
                    BookingId = booking.Id,
                    PartnerId = partnerId,
                    IsConfirmed = isConfirmed
                };
                _context.SingleConfirmations.Add(singleConf);

                if (!isConfirmed)
                {
                    allPartnersConfirmed = false;
                }
            }
            await _context.SaveChangesAsync();

            if (!allPartnersConfirmed)
            {
                booking.Status = BookingStatus.Failed;
                booking.FailureReason = "One or more partners rejected the booking.";
                await _context.SaveChangesAsync();
                return Conflict(new { Message = booking.FailureReason, Confirmations = booking.PartnerConfirmations });
            }

            // Резервация мест в туре
            booking.Status = BookingStatus.PendingPayment;
            bool seatsReserved = await _tourService.ReserveSeatAsync(request.TourId, request.TouristsNumber, token);

            if (!seatsReserved)
            {
                booking.Status = BookingStatus.Failed;
                booking.FailureReason = "Failed to reserve seats in Tour Service.";
                await _context.SaveChangesAsync();
                return Conflict(booking.FailureReason);
            }

            if (request.CartItemId != null)
            {
                var cartItemToRemove = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == request.CartItemId);
                _context.CartItems.Remove(cartItemToRemove);
            }

            await _context.SaveChangesAsync();
            return Ok(new
            {
                Message = "Booking created. Waiting for payment.",
                BookingId = booking.Id,
                AmountToPay = booking.TotalAmount,
                Status = booking.Status.ToString()
            });

        }

        // POST: api/booking/{id}/pay
        [HttpPost("{id}/pay")]
        public async Task<IActionResult> PayForBooking(int id)
        {
            var currentUserId = GetUserId();
            var token = GetToken();
            if (currentUserId == null || token == null) return Unauthorized();

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound("Booking not found.");

            bool isStaff = User.IsInRole("Admin") || User.IsInRole("Manager");
            if (booking.UserId != currentUserId && !isStaff) return Forbid();

            if (booking.Status == BookingStatus.Confirmed)
                return BadRequest("Booking is already paid.");

            if (booking.Status != BookingStatus.PendingPayment)
                return BadRequest($"Cannot pay for booking with status {booking.Status}.");

            var payReq = new PaymentRequest
            {
                BookingId = booking.Id,
                UserId = booking.UserId,
                Amount = booking.TotalAmount
            };

            var paymentResult = await _paymentService.ProcessPaymentAsync(payReq, token);

            if (paymentResult != null && paymentResult.Status == "Success")
            {
                booking.Status = BookingStatus.Confirmed;
                booking.PaymentId = paymentResult.Id;
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Payment successful. Booking confirmed.", BookingId = booking.Id });
            }
            else
            {
                return BadRequest(new
                {
                    Message = "Payment failed. Please try again.",
                    Reason = paymentResult?.FailureReason ?? "Unknown error"
                });
            }
        }

        // GET: api/booking/me
        [HttpGet("me")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = GetUserId();
            var bookings = await _context.Bookings
                .Include(b => b.PartnerConfirmations)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
            return Ok(bookings);
        }

        // POST: api/booking/cancel/{id}
        [HttpPost("cancel/{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userId = GetUserId();
            var token = GetToken();
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null) return NotFound();

            bool isStaff = User.IsInRole("Admin") || User.IsInRole("Manager");
            if (booking.UserId != userId && !isStaff) return Forbid();

            if (booking.Status == BookingStatus.Cancelled || booking.Status == BookingStatus.Failed)
                return BadRequest("Booking cannot be cancelled.");

            if (booking.Status == BookingStatus.Confirmed && booking.PaymentId.HasValue)
            {
                var refundReq = new RefundRequest
                {
                    PaymentId = booking.PaymentId.Value,
                    Reason = "User requested cancellation of booking."
                };

                var refundResponse = await _paymentService.RefundPaymentAsync(refundReq, token);

                if (refundResponse == null || !refundResponse.IsSuccess)
                {
                    var errorMsg = refundResponse?.Message ?? "Unknown error during refund";
                    return StatusCode(500, $"Failed to refund payment: {errorMsg}");
                }
            }

            if (booking.Status == BookingStatus.Confirmed || booking.Status == BookingStatus.PendingPayment)
            {
                await _tourService.ReleaseSeatAsync(booking.TourId, booking.NumberOfSeats, token);
            }

            // 3. Обновление статуса
            booking.Status = BookingStatus.Cancelled;
            booking.FailureReason = "Cancelled by user/manager.";
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Booking cancelled successfully." });
        }

        // GET: api/booking/stats/my-countries
        [HttpGet("stats/my-countries")]
        public async Task<IActionResult> GetMyCountryStatistics()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var myBookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .Where(b => b.Status == BookingStatus.Confirmed)
                .ToListAsync();

            if (!myBookings.Any())
            {
                return Ok(new List<object>());
            }

            var tourIds = myBookings.Select(b => b.TourId).Distinct();

            var countryStats = new Dictionary<string, int>();

            foreach (var tourId in tourIds)
            {
                var tour = await _tourService.GetTourAsync(tourId);

                if (tour != null && !string.IsNullOrEmpty(tour.Location))
                {
                    var locationParts = tour.Location.Split(',');
                    var country = locationParts.First().Trim();

                    int timesVisited = myBookings.Count(b => b.TourId == tourId);

                    if (countryStats.ContainsKey(country))
                    {
                        countryStats[country] += timesVisited;
                    }
                    else
                    {
                        countryStats.Add(country, timesVisited);
                    }
                }
            }

            var result = countryStats.Select(x => new
            {
                Country = x.Key,
                VisitCount = x.Value
            }).OrderByDescending(x => x.VisitCount);

            return Ok(result);
        }
    }
}