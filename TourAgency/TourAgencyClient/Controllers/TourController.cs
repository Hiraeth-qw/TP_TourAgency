using Microsoft.AspNetCore.Mvc;
using TourAgencyClient.Services;
using TourAgencyClient.Models;

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