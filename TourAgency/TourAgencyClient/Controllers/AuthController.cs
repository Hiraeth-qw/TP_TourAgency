using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using TourAgencyClient.DTOs;
using TourAgencyClient.Models;
using TourAgencyClient.Services;

namespace TourAgencyClient.Controllers
{
    public class AuthController : Controller
    {
        private readonly IBaseService _baseService;

        public AuthController(IBaseService baseService)
        {
            _baseService = baseService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var resp = await _baseService.SendAsync<AuthResponseDto>("UserApi", HttpMethod.Post, "/api/account/login", model);
            var result = resp.Result;

            if (result != null && !string.IsNullOrEmpty(result.Token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(result.Token);
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaims(jwt.Claims);

                var props = new AuthenticationProperties();
                props.StoreTokens(new List<AuthenticationToken>
                {
                    new AuthenticationToken { Name = "access_token", Value = result.Token }
                });

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _baseService.SendAsync<object>("UserApi", HttpMethod.Post, "/api/account/register", model);

            if (result.IsSuccess)
            {
                TempData["ToastMessage"] = "Регистрация прошла успешно! Теперь вы можете войти.";
                TempData["ToastType"] = "Success";
                return RedirectToAction("Login", "Auth");
            }
            else
            {
                string errorMessage = result.ErrorMessage ?? "Произошла неизвестная ошибка при регистрации.";

                if (result.StatusCode == 400)
                {
                    ModelState.AddModelError("", "Пользователь с таким Email уже существует.");
                }
                else
                {
                    ModelState.AddModelError("", errorMessage);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}