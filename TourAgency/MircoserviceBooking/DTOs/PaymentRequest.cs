namespace MicroserviceBooking.DTOs
{
    public class PaymentRequest
    {
        public int BookingId { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
    }
}
