using MicroserviceBooking.DTOs;

namespace MicroserviceBooking.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request, string token);
        Task<RefundResponse> RefundPaymentAsync(RefundRequest request, string token);
    }
}