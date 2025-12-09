using Microsoft.AspNetCore.Mvc;
using TourAgencyClient.DTOs;
using TourAgencyClient.Services;

namespace TourAgencyClient.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;
        private readonly BookingService _bookingService;

        public AccountController(AccountService accountService, BookingService bookingService)
        {
            _accountService = accountService;
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userProfile = await _accountService.GetMyProfileAsync();
            if (userProfile == null) return RedirectToAction("Login", "Auth");

            userProfile.CountryStats = await _bookingService.GetMyStatsAsync();

            return View(userProfile);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userProfile = await _accountService.GetMyProfileAsync();

            if (userProfile == null) return RedirectToAction("Login", "Auth");

            var editDto = new ProfileEdit
            {
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                PhoneNumber = userProfile.PhoneNumber
            };

            return View(editDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileEdit model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? phoneNumberToSend = string.IsNullOrWhiteSpace(model.PhoneNumber) ? null : model.PhoneNumber;

            var patchOperations = new List<JsonPatchOperation>
            {
                new JsonPatchOperation { Op = "replace", Path = "/firstName", Value = model.FirstName },
                new JsonPatchOperation { Op = "replace", Path = "/lastName", Value = model.LastName },
                new JsonPatchOperation { Op = "replace", Path = "/phoneNumber", Value = phoneNumberToSend }
            };

            var response = await _accountService.EditProfileAsync(patchOperations);

            if (response.IsSuccess)
            {
                TempData["ToastMessage"] = "Профиль успешно обновлен.";
                TempData["ToastType"] = "Success";
                return RedirectToAction("Profile");
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.ErrorMessage ?? "Ошибка при сохранении данных.");
                return View(model);
            }
        }
    }
}
