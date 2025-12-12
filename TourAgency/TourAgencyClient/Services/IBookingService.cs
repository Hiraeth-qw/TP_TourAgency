using TourAgencyClient.DTOs;
using TourAgencyClient.Models;

namespace TourAgencyClient.Services
{
    public interface IBookingService: IBaseService
    {
        Task<bool> AddToCartAsync(AddItemToCartDto dto);
        Task<ResponseDto<object>> CancelBookingAsync(int id);
        Task<ResponseDto<object>> CreateBookingAsync(CreateBookingRequest dto);
        Task<List<BookingViewModel>> GetMyBookingsAsync();
        Task<List<CartItemViewModel>> GetMyCartAsync();
        Task<List<CountryStatsViewModel>> GetMyStatsAsync();
        Task<ResponseDto<object>> PayForBookingAsync(int id);
        Task<bool> RemoveFromCartAsync(int id);
    }
}