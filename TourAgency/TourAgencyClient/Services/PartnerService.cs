using TourAgencyClient.Services;
using System.Net.Http;
using TourAgencyClient.DTOs;

namespace TourAgencyClient.Services
{
    public class PartnerService : BaseService
    {
        private const string ClientName = "PartnerApi";

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

            var tasks = partnerIds.Select(id =>
                SendAsync<PartnerNameDto>(ClientName, HttpMethod.Get, $"/api/partners/{id}")
            ).ToList();

            var results = await Task.WhenAll(tasks);

            foreach (var result in results)
            {
                if (result != null && !string.IsNullOrEmpty(result.Name))
                {
                    partnerNames.Add(result.Name);
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