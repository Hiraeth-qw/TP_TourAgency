using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;

namespace TourAgencyClient.Services
{
    public class BaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<T?> SendAsync<T>(string clientName, HttpMethod method, string url, object? data = null)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            var message = new HttpRequestMessage(method, url);

            if (data != null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            }

            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.SendAsync(message);

            var apiContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(apiContent);
            }

            Console.WriteLine($"API Error: {response.StatusCode} - {apiContent}");
            return default;
        }
    }
}
