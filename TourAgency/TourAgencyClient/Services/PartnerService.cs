using TourAgencyClient.Services;
using System.Net.Http;
using TourAgencyClient.DTOs;

namespace TourAgencyClient.Services
{
    public class PartnerService : BaseService
    {
        public PartnerService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
            : base(httpClientFactory, httpContextAccessor)
        {
        }

        public async Task<List<string>> GetPartnerNamesByIdsAsync(List<int> partnerIds)
        {
            if (partnerIds == null || !partnerIds.Any())
            {
                return new List<string>();
            }

            var partnerNames = new List<string>();

            var tasks = partnerIds.Select(id => SendAsync<PartnerNameDto>("PartnerApi", HttpMethod.Get, $"/api/partners/{id}")).ToList();
            

            var results = await Task.WhenAll(tasks);

            foreach (var response in results)
            {
                if (response.IsSuccess && response.Result != null && !string.IsNullOrEmpty(response.Result.Name))
                {
                    partnerNames.Add(response.Result.Name);
                }
                else
                {
                    partnerNames.Add("Неизвестный партнер");
                }
            }

            return partnerNames;
        }
    }
}