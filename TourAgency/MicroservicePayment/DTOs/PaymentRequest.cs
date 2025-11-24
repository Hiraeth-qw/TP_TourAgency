using System.ComponentModel.DataAnnotations;

namespace MicroservicePayment.DTOs
{
    public class PaymentRequest
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
    }
}
