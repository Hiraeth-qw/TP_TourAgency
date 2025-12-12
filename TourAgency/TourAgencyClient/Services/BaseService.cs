using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using TourAgencyClient.DTOs;

namespace TourAgencyClient.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<T>> SendAsync<T>(string clientName, HttpMethod method, string url, object? data = null)
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

            var responseDto = new ResponseDto<T>
            {
                StatusCode = (int)response.StatusCode
            };

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    responseDto.Result = JsonConvert.DeserializeObject<T>(apiContent);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON Deserialization Error: {ex.Message}");
                    responseDto.IsSuccess = false;
                    responseDto.ErrorMessage = $"Failed to deserialize response: {ex.Message}";
                }
            }
            else
            {
                responseDto.IsSuccess = false;
                responseDto.ErrorMessage = $"API вернул ошибку: {response.StatusCode}. Подробности: {apiContent}";
            }
            return responseDto;
        }
    }
}
