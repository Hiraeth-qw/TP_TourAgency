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
    }
}
