using TourAgencyClient.DTOs;
using TourAgencyClient.Models;

namespace TourAgencyClient.Services
{
    public interface IUserService: IBaseService
    {
        Task<ResponseDto<object>> AssignRoleAsync(AssignRole model);
        Task<ResponseDto<object>> DeleteUserAsync(string id);
        Task<List<ClientUserViewModel>> GetAllClientsAsync();
        Task<List<UserReadDto>> GetAllUsersAsync();
        Task<UserReadDto?> GetUserByIdAsync(string id);
        Task<ResponseDto<object>> RemoveRoleAsync(AssignRole model);
    }
}