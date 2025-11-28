using System.Security.Claims;
using MicroserviceBooking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicroserviceBooking.DTOs;
using MicroserviceBooking.Services;

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
            bool seatsReserved = await _tourService.ReserveSeatAsync(request.TourId, token);

            if (!seatsReserved)
            {
                booking.Status = BookingStatus.Failed;
                booking.FailureReason = "Failed to reserve seats in Tour Service.";
                await _context.SaveChangesAsync();
                return Conflict(booking.FailureReason);
            }

            // Оплата
            var payReq = new PaymentRequest
            {
                BookingId = booking.Id,
                UserId = targetUserId,
                Amount = booking.TotalAmount
            };

            var paymentResult = await _paymentService.ProcessPaymentAsync(payReq, token);

            if (paymentResult != null && paymentResult.Status == "Success")
            {
                booking.Status = BookingStatus.Confirmed;
                booking.PaymentId = paymentResult.Id;
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Booking Confirmed", BookingId = booking.Id });
            }
            else
            {
                booking.Status = BookingStatus.Failed;
                booking.FailureReason = paymentResult?.FailureReason ?? "Payment failed.";
                await _context.SaveChangesAsync();
                return BadRequest(new { Message = "Payment failed", Reason = booking.FailureReason });
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
    }
}