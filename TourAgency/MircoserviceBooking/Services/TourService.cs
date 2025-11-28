using System.Net.Http.Headers;
using MicroserviceBooking.DTOs;

namespace MicroserviceBooking.Services
{
    public class TourService
    {
        private readonly HttpClient _client;
        public TourService(HttpClient client)
        {
            _client = client;
        }

        public async Task<ReadTour?> GetTourAsync(int id) => await _client.GetFromJsonAsync<ReadTour>($"{id}");

        public async Task<bool> ReserveSeatAsync(int id, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PatchAsync($"{id}/reserve-seat", null);
            return response.IsSuccessStatusCode;
        }
    }
}
