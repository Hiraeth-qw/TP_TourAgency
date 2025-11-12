using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MicroserviceUser.Models;
using MicroserviceUser.DTOs;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;


namespace MicroserviceUser.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<User> userManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        //POST: api/account/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                firstName = model.FirstName,
                lastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                string roleName = Roles.Client.ToString();

                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }

                await _userManager.AddToRoleAsync(user, roleName);

                return Ok(new { Message = "User registered successfully." });
            }

            return BadRequest(result.Errors);
        }

        //POST: api/account/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtOptions:Key"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JwtOptions:Issuer"],
                    audience: _configuration["JwtOptions:Audience"],
                    expires: DateTime.UtcNow.AddHours(2),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    userId = user.Id,
                    email = user.Email,
                    roles = userRoles
                });
            }

            return Unauthorized(new { Message = "Invalid login or password." });
        }

        //POST: api/account/assign-role
        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRole model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return NotFound($"User with email {model.Email} was not found.");

            if (!Enum.TryParse(model.RoleName, true, out Roles roleEnum))
                return BadRequest($"Role {model.RoleName} does not exist");

            if (!await _roleManager.RoleExistsAsync(model.RoleName))
                await _roleManager.CreateAsync(new IdentityRole(model.RoleName));

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);

            if (result.Succeeded)
                return Ok($"Successfully assigned  role '{model.RoleName} to user '{model.Email}'.");

            return BadRequest(result.Errors);
        }

        //POST: api/account/edit-profile
        [HttpPatch("edit-profile")]
        [Authorize]
        public async Task<IActionResult> EditProfile([FromBody] JsonPatchDocument<ProfileEdit> patch)
        {
            if (patch == null)
            {
                return BadRequest("Patch is null.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var userDto = new ProfileEdit
            {
                FirstName = user.firstName,
                LastName = user.lastName
            };

            patch.ApplyTo(userDto, ModelState);

            if (!TryValidateModel(userDto))
            {
                return BadRequest(ModelState);
            }

            user.firstName = userDto.FirstName;
            user.lastName = userDto.LastName;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return NoContent();
            }

            return BadRequest(result.Errors);
        }
    }
}