namespace MicroservicePayment.DTOs
{
    public class PaymentResponse
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string? FailureReason { get; set; }
    }
}
