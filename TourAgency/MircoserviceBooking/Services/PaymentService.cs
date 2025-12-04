using System.Net.Http.Headers;
using MicroserviceBooking.DTOs;

namespace MicroserviceBooking.Services
{
    public class PaymentService
    {
        private readonly HttpClient _client;
        public PaymentService(HttpClient client) => _client = client;

        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsJsonAsync("process", request);

            return await response.Content.ReadFromJsonAsync<PaymentResponse>();
        }

        public async Task<RefundResponse> RefundPaymentAsync(RefundRequest request, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsJsonAsync("refund", request);

            return await response.Content.ReadFromJsonAsync<RefundResponse>();
        }
    }
}
