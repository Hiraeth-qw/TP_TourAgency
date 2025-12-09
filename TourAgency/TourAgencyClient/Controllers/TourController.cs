using Microsoft.AspNetCore.Mvc;
using TourAgencyClient.DTOs;
using TourAgencyClient.Models;
using TourAgencyClient.Services;

namespace TourAgencyClient.Controllers
{
    public class TourController : Controller
    {
        private readonly TourService _tourService;

        public TourController(TourService tourService)
        {
            _tourService = tourService;
        }

        [HttpGet]
        public async Task<IActionResult> List(TourSearchQuery query)
        {
            ViewData["CurrentSearchQuery"] = query;

            if (query.Location != null || query.StartDate.HasValue)
            {
                ViewData["SearchQuery"] = $"Поиск: Место '{query.Location ?? "любое"}', Дата с '{query.StartDate?.ToString("dd.MM.yyyy") ?? "любая"}'";
            }
            else
            {
                ViewData["SearchQuery"] = "Все доступные туры.";
            }

            var tours = await _tourService.GetToursAsync(query);

            return View("ToursList", tours);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var tourDetails = await _tourService.GetTourByIdAsync(id);

            if (tourDetails == null)
            {
                return NotFound("К сожалению, тур с ID: " + id + " не найден.");
            }

            return PartialView("_TourDetailsModal", tourDetails);
        }
    }
}