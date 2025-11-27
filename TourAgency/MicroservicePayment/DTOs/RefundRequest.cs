using System.ComponentModel.DataAnnotations;

namespace MicroservicePayment.DTOs
{
    public class RefundRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int PaymentId { get; set; }
        public string? Reason { get; set; }
    }
}
