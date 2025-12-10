using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourAgencyClient.DTOs;
using TourAgencyClient.Services;

namespace TourAgencyClient.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly BookingService _bookingService;

        public BookingController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddItemToCartDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Некорректно указано количество мест." });
            }

            var success = await _bookingService.AddToCartAsync(dto);

            if (success)
            {
                return Ok(new { message = "Тур успешно добавлен в корзину." });
            }
            else
            {
                return StatusCode(500, new { message = "Не удалось добавить тур в корзину. Проверьте, доступен ли тур и места." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            var cartItems = await _bookingService.GetMyCartAsync();
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
        {
            if (!ModelState.IsValid) return BadRequest("Некорректные данные.");

            var response = await _bookingService.CreateBookingAsync(request);

            if (response.IsSuccess)
            {
                return Ok(new { message = "Бронирование успешно создано! Ожидается оплата." });
            }
            else
            {
                string errorMsg = response.ErrorMessage ?? "Ошибка при создании бронирования.";
                if (response.StatusCode == 409)
                    errorMsg = "К сожалению, места закончились или партнер отклонил запрос.";

                return StatusCode(response.StatusCode == 0 ? 500 : response.StatusCode, new { message = errorMsg });
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyBookings()
        {
            var bookings = await _bookingService.GetMyBookingsAsync();
            return View(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> Pay(int id)
        {
            var response = await _bookingService.PayForBookingAsync(id);
            if (response.IsSuccess)
                return Ok(new { message = "Оплата прошла успешно!" });

            return BadRequest(new { message = response.ErrorMessage ?? "Ошибка оплаты." });
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var response = await _bookingService.CancelBookingAsync(id);
            if (response.IsSuccess)
                return Ok(new { message = "Бронирование отменено." });

            return BadRequest(new { message = response.ErrorMessage ?? "Ошибка отмены." });
        }
    }
}