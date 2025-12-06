using TourAgencyClient.DTOs;

namespace TourAgencyClient.Services
{
    public class BookingService: BaseService
    {
        public BookingService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
            : base(httpClientFactory, httpContextAccessor)
        {
        }

        public async Task<bool> AddToCartAsync(AddItemToCartDto dto)
        {
            var result = await SendAsync<object>("BookingApi", HttpMethod.Post, "api/booking/plan/add", dto);

            return result != null;
        }
    }
}
