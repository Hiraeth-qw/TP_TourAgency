using System.Net.Http.Headers;
using MicroserviceBooking.DTOs;

namespace MicroserviceBooking.Services
{
    public class TourService : ITourService
    {
        private readonly HttpClient _client;
        public TourService(HttpClient client)
        {
            _client = client;
        }

        public async Task<ReadTour?> GetTourAsync(int id) => await _client.GetFromJsonAsync<ReadTour>($"{id}");

        public async Task<bool> ReserveSeatAsync(int id, int numberOfSeats, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var requestData = new { Quantity = numberOfSeats };
            var response = await _client.PatchAsJsonAsync($"{id}/reserve-seat", requestData);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ReleaseSeatAsync(int id, int numberOfSeats, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var requestData = new { Quantity = numberOfSeats };
            var response = await _client.PatchAsJsonAsync($"{id}/release-seat", requestData);
            return response.IsSuccessStatusCode;
        }
    }
}
