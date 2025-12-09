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

        private string StatusTranslation(string status)
        {
            return status switch
            {
                "Confirmed" => "Подтверждено",
                "PendingPayment" => "Ожидание оплаты",
                "Cancelled" => "Отменено",
                "Failed" => "Ошибка",
                "PendingPartners" => "Ожидание партнеров",
            };
        }

        public async Task<List<BookingViewModel>> GetMyBookingsAsync()
        {
            var resp = await SendAsync<List<BookingDto>>("BookingApi", HttpMethod.Get, "/api/booking/me");
            var bookings = resp.Result;

            var result = new List<BookingViewModel>();
            if (bookings == null) return result;

            foreach (var b in bookings)
            {
                var tour = await _tourService.GetTourByIdAsync(b.TourId);

                result.Add(new BookingViewModel
                {
                    Id = b.Id,
                    TourId = b.TourId,
                    TourTitle = tour?.Title ?? "Неизвестный тур",
                    TourStartDate = tour.StartDate,
                    TourEndDate = tour.EndDate,
                    BookingDate = b.BookingDate,
                    NumberOfSeats = b.NumberOfSeats,
                    TotalAmount = b.TotalAmount,
                    Status = StatusTranslation(b.Status)
                });
            }
            return result;
        }

        public async Task<List<CountryStatsViewModel>> GetMyStatsAsync()
        {
            var resp = await SendAsync<List<CountryStatsViewModel>>("BookingApi", HttpMethod.Get, "/api/booking/stats/my-countries");
            var stats = resp.Result;

            return stats ?? new List<CountryStatsViewModel>();
        }

        public async Task<ResponseDto<object>> PayForBookingAsync(int id)
        {
            return await SendAsync<object>("BookingApi", HttpMethod.Post, $"/api/booking/{id}/pay");
        }

        public async Task<ResponseDto<object>> CancelBookingAsync(int id)
        {
            return await SendAsync<object>("BookingApi", HttpMethod.Post, $"/api/booking/cancel/{id}");
        }
    }
}
