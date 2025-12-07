using TourAgencyClient.DTOs;
using TourAgencyClient.Models;

namespace TourAgencyClient.Services
{
    public class BookingService: BaseService
    {
        private readonly TourService _tourService;
        public BookingService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, TourService tourService)
            : base(httpClientFactory, httpContextAccessor)
        {
            _tourService = tourService;
        }

        public async Task<bool> AddToCartAsync(AddItemToCartDto dto)
        {
            var resp = await SendAsync<object>("BookingApi", HttpMethod.Post, "api/booking/plan/add", dto);
            var result = resp.Result;

            return result != null;
        }

        public async Task<List<CartItemViewModel>> GetMyCartAsync()
        {
            var resp = await SendAsync<List<CartItemDto>>("BookingApi", HttpMethod.Get, "/api/booking/plan");
            var cartItems = resp.Result;

            if (!resp.IsSuccess || cartItems    == null)
            {
                return new List<CartItemViewModel>();
            }

            return cartItems.Select(item => new CartItemViewModel
            {
                CartItemId = item.CartItemId,
                TourId = item.TourId,
                TouristsNumber = item.NumberOfSeats,
                Title = item.Title,
                Location = item.Location,
                Price = item.Price,
                TotalPrice = item.TotalPrice,
                StartDate = item.StartDate,
                AddedDate = item.AddedDate
            }).ToList();
        }

        public async Task<ResponseDto<object>> CreateBookingAsync(CreateBookingRequest dto)
        {
            return await SendAsync<object>("BookingApi", HttpMethod.Post, "/api/booking", dto);
        }
    }
}
