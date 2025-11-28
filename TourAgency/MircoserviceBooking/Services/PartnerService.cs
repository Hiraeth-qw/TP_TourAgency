using System.Net.Http.Headers;
using MicroserviceBooking.DTOs;

namespace MicroserviceBooking.Services
{
    public class PartnerService
    {
        private readonly HttpClient _client;
        public PartnerService(HttpClient client) => _client = client;

        public async Task<bool> ConfirmBookingAsync(PartnerConfirmationRequest request, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsJsonAsync("confirm-booking", request);
            if (!response.IsSuccessStatusCode) return false;
            return await response.Content.ReadFromJsonAsync<bool>();
        }
    }
}
