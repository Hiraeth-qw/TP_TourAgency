using System.ComponentModel.DataAnnotations;

namespace MicroserviceBooking.DTOs
{
    public class RefundRequest
    {
        [Required]
        public int PaymentId { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;
    }
}
