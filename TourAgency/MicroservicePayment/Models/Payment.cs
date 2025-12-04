using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroservicePayment.Models
{
    public enum PaymentStatus
    {
        Pending,
        Success,
        Failed,
        Refunded
    }

    public class Payment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string UserId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow.AddHours(3);
        public PaymentStatus Status { get; set; }
        public string? FailureReason { get; set; }
    }
}