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
    }
}
