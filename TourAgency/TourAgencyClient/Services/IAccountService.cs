using TourAgencyClient.DTOs;
using TourAgencyClient.Models;

namespace TourAgencyClient.Services
{
    public interface IAccountService: IBaseService
    {
        Task<ResponseDto<object>> EditProfileAsync(List<JsonPatchOperation> patchOperations);
        Task<UserProfileViewModel?> GetMyProfileAsync();
    }
}