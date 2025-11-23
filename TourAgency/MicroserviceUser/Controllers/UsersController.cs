using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicroserviceUser.DTOs;
using MicroserviceUser.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroserviceUser.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // 1. GET: api/users
        [HttpGet]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserReadDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserReadDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.firstName,
                    LastName = user.lastName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles
                });
            }

            return Ok(userDtos);
        }

        // 2. GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserReadDto>> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var currentUserRoles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value);
            
            if (currentUserId != user.Id && !currentUserRoles.Contains("Admin") && !currentUserRoles.Contains("Manager"))
            {
                return Forbid();
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new UserReadDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.firstName,
                LastName = user.lastName,
                PhoneNumber = user.PhoneNumber,
                Roles = roles
            });
        }

        // 3. GET: api/users/me
        [HttpGet("me")]
        public async Task<ActionResult<UserReadDto>> GetMyProfile()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            return await GetUserById(userId);
        }
    }
}