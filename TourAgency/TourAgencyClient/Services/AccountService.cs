using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.JsonPatch.Operations;
using TourAgencyClient.DTOs;
using TourAgencyClient.Models;

namespace TourAgencyClient.Services
{
    public class AccountService: BaseService
    {
        public AccountService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
            : base(httpClientFactory, httpContextAccessor)
        {
        }

        public async Task<UserProfileViewModel?> GetMyProfileAsync()
        {
            var resp = await SendAsync<UserProfileViewModel>("UserApi", HttpMethod.Get, "/api/users/me");
            return resp.Result;
        }

        public async Task<ResponseDto<object>> EditProfileAsync(List<JsonPatchOperation> patchOperations)
        {
            var response = await SendAsync<object>("UserApi", HttpMethod.Patch, "/api/account/edit-profile", patchOperations);
            return response;
        }
    }
}
