using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourAgencyClient.DTOs;
using TourAgencyClient.Models;
using TourAgencyClient.Services;

namespace TourAgencyClient.Controllers
{
    public class TourController : Controller
    {
        private readonly ITourService _tourService;
        private readonly IUserService _userService;

        public TourController(ITourService tourService, IUserService userService)
        {
            _tourService = tourService;
            _userService = userService;
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

            if (User.IsInRole("Manager") || User.IsInRole("Admin"))
            {
                var clients = await _userService.GetAllClientsAsync();
                ViewData["ClientsList"] = clients;
            }

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

        [HttpGet]
        [Authorize(Roles = "ContentManager")]
        public IActionResult Create()
        {
            return View("TourForm", new TourFormViewModel
            {
                StartDate = DateTime.Today.AddDays(2),
                EndDate = DateTime.Today.AddDays(15)
            });
        }

        [HttpGet]
        [Authorize(Roles = "ContentManager")]
        public async Task<IActionResult> Edit(int id)
        {
            var tour = await _tourService.GetTourByIdAsync(id);
            if (tour == null) return NotFound();

            var viewModel = new TourFormViewModel
            {
                Id = tour.Id,
                Title = tour.Title,
                Location = tour.Location,
                Description = tour.Description,
                StartDate = tour.StartDate,
                EndDate = tour.EndDate,
                Price = tour.Price,
                AvailableSeats = tour.AvailableSeats,
                PartnerIdsString = tour.PartnerIds != null ? string.Join(", ", tour.PartnerIds) : ""
            };

            return View("TourForm", viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "ContentManager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(TourFormViewModel model)
        {
            if (!ModelState.IsValid) return View("TourForm", model);

            model.PartnerIds = new List<int>();
            if (!string.IsNullOrWhiteSpace(model.PartnerIdsString))
            {
                model.PartnerIds = model.PartnerIdsString.Split(',').Select(s => int.TryParse(s.Trim(), out int n) ? n : 0)
                    .Where(n => n > 0)
                    .ToList();
            }

            ResponseDto<object> response;

            if (model.Id.HasValue && model.Id.Value > 0)
            {
                response = await _tourService.UpdateTourAsync(model.Id.Value, model);
            }
            else
            {
                response = await _tourService.CreateTourAsync(model);
            }

            if (response.IsSuccess)
            {
                TempData["ToastMessage"] = model.Id.HasValue ? "Тур обновлен." : "Тур создан.";
                TempData["ToastType"] = "Success";
                return RedirectToAction("List", new { location = "", startDate = "" });
            }

            ModelState.AddModelError("", response.ErrorMessage ?? "Ошибка сохранения.");
            return View("TourForm", model);
        }

        [HttpPost]
        [Authorize(Roles = "ContentManager")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _tourService.DeleteTourAsync(id);

            if (response.IsSuccess)
                return Ok(new { message = "Тур удален." });

            return BadRequest(new { message = response.ErrorMessage ?? "Не удалось удалить тур." });
        }
    }
}