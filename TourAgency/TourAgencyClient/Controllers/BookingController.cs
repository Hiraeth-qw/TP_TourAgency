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
    }
}