namespace MicroservicePayment.DTOs
{
    public class RefundResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
        public DateTime RefundedAt { get; set; }
    }
}
