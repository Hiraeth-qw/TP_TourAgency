namespace MicroserviceBooking.Models
{
    public enum BookingStatus
    {
        PendingPartners,
        PendingPayment,
        Confirmed,
        Failed,
        Cancelled
    }

    public class Booking
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int TourId { get; set; }
        public int NumberOfSeats { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public BookingStatus Status { get; set; } = BookingStatus.PendingPartners;
        public int? PaymentId { get; set; }
        public string? FailureReason { get; set; }
        public List<SingleConfirmation> PartnerConfirmations { get; set; } = new();
    }
}