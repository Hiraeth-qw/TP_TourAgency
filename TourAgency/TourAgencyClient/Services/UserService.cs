using TourAgencyClient.DTOs;
using TourAgencyClient.Models;

namespace TourAgencyClient.Services
{
    public class UserService : BaseService
    {
        public UserService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
            : base(httpClientFactory, httpContextAccessor)
        {
        }

        public async Task<List<UserReadDto>> GetAllUsersAsync()
        {
            var result = await SendAsync<List<UserReadDto>>("UserApi", HttpMethod.Get, "/api/users");
            return result.Result ?? new List<UserReadDto>();
        }

        public async Task<List<ClientUserViewModel>> GetAllClientsAsync()
        {
            var allUsers = await SendAsync<List<UserReadDto>>("UserApi", HttpMethod.Get, "/api/users");

            if (allUsers.IsSuccess && allUsers.Result != null)
            {
                return allUsers.Result
                    .Where(u => u.Roles.Contains("Client"))
                    .Select(u => new ClientUserViewModel
                    {
                        Id = u.Id,
                        DisplayName = $"{u.FirstName} {u.LastName} ({u.Email})"
                    })
                    .ToList();
            }
            return new List<ClientUserViewModel>();
        }

        public async Task<UserReadDto?> GetUserByIdAsync(string id)
        {
            var result = await SendAsync<UserReadDto>("UserApi", HttpMethod.Get, $"/api/users/{id}");
            return result.Result;
        }

        public async Task<ResponseDto<object>> DeleteUserAsync(string id)
        {
            return await SendAsync<object>("UserApi", HttpMethod.Delete, $"/api/users/{id}");
        }

        public async Task<ResponseDto<object>> AssignRoleAsync(AssignRole model)
        {
            return await SendAsync<object>("UserApi", HttpMethod.Post, "/api/account/assign-role", model);
        }

        public async Task<ResponseDto<object>> RemoveRoleAsync(AssignRole model)
        {
            return await SendAsync<object>("UserApi", HttpMethod.Post, "api/account/remove-role", model);
        }
    }
}
