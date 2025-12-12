using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourAgencyClient.DTOs;
using TourAgencyClient.Services;

namespace TourAgencyClient.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController: Controller
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Users(string searchString)
        {
            var users = await _userService.GetAllUsersAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                users = users.Where(u =>
                    (u.FirstName != null && u.FirstName.ToLower().Contains(searchString)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(searchString)) ||
                    (u.Id.ToLower().Contains(searchString)) ||
                    (u.Email.ToLower().Contains(searchString)) ||
                    (u.RoleString.Contains(searchString))
                ).ToList();
            }

            ViewData["CurrentFilter"] = searchString;

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> UserDetails(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRole(AssignRole model)
        {
            var response = await _userService.AssignRoleAsync(model);

            if (response.IsSuccess)
            {
                TempData["ToastMessage"] = $"Роль '{model.RoleName}' успешно назначена пользователю {model.Email}.";
                TempData["ToastType"] = "Success";
            }
            else
            {
                TempData["ToastMessage"] = response.ErrorMessage ?? "Ошибка при смене роли.";
                TempData["ToastType"] = "Error";
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRole(string email, string roleName)
        {
            var model = new AssignRole { Email = email, RoleName = roleName };
            var response = await _userService.RemoveRoleAsync(model);

            if (response.IsSuccess)
            {
                TempData["ToastMessage"] = $"Роль '{roleName}' удалена у пользователя {email}.";
                TempData["ToastType"] = "Success";
            }
            else
            {
                TempData["ToastMessage"] = response.ErrorMessage ?? "Ошибка при удалении роли.";
                TempData["ToastType"] = "Error";
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == id)
            {
                TempData["ToastMessage"] = "Вы не можете удалить собственного пользователя!";
                TempData["ToastType"] = "Error";
                return RedirectToAction("Users");
            }

            var response = await _userService.DeleteUserAsync(id);

            if (response.IsSuccess)
            {
                TempData["ToastMessage"] = "Пользователь удален.";
                TempData["ToastType"] = "Success";
            }
            else
            {
                TempData["ToastMessage"] = response.ErrorMessage ?? "Ошибка удаления.";
                TempData["ToastType"] = "Error";
            }

            return RedirectToAction("Users");
        }
    }
}
