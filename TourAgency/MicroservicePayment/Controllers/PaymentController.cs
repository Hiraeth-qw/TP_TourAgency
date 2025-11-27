using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MicroservicePayment.DTOs;
using MicroservicePayment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MicroservicePayment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentContext _context;

        public PaymentController(PaymentContext context)
        {
            _context = context;
        }
        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // POST: api/Payment/process
        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
            var payment = new Payment
            {
                BookingId = request.BookingId,
                UserId = request.UserId,
                Amount = request.Amount,
                Status = PaymentStatus.Pending
            };

            await Task.Delay(2000);

            bool isSuccess = true;
            string? failReason = null;

            //Имитация ошибки для тестирования
            if (request.Amount == 6666)
            {
                isSuccess = false;
                failReason = "Bank connection error";
            }

            if (isSuccess)
            {
                payment.Status = PaymentStatus.Success;
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
                payment.FailureReason = failReason;
            }

            _context.Payment.Add(payment);
            await _context.SaveChangesAsync();

            if (isSuccess)
            {
                return Ok(payment);
            }
            else
            {
                return BadRequest(payment);
            }
        }

        // GET: api/Payment/me
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Payment>>> GetMyPayments()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var payments = await _context.Payment
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.Date)
                .ToListAsync();

            if (!payments.Any())
            {
                return NotFound("No payments found for this user.");
            }

            return Ok(payments);
        }

        //GET: api/Payment/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetPayment(int id)
        {
            var payment = await _context.Payment.FindAsync(id);

            if (payment == null)
            {
                return NotFound();
            }

            var currentUserId = GetCurrentUserId();
            bool isAdminOrManager = User.IsInRole("Admin") || User.IsInRole("Manager");
            bool isOwner = payment.UserId == currentUserId;
            if (!isAdminOrManager && !isOwner)
            {
                return Forbid();
            }

            return Ok(payment);
        }

        // POST: api/payment/refund
        [HttpPost("refund")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> RefundPayment([FromBody] RefundRequest request)
        {
            var payment = await _context.Payment.FindAsync(request.PaymentId);

            if (payment == null)
            {
                return NotFound($"Payment with ID {request.PaymentId} not found.");
            }

            if (payment.Status != PaymentStatus.Success)
            {
                return Conflict($"Cannot refund payment. Current status is {payment.Status}.");
            }

            payment.Status = PaymentStatus.Refunded;
            payment.FailureReason = $"Reason: {request.Reason ?? "Not specified"}.";

            _context.Payment.Update(payment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Payment successfully refunded.",
                NewStatus = payment.Status.ToString()
            });
        }
    }
}