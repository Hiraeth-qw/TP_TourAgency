using System.Web;
using Microsoft.AspNetCore.WebUtilities;
using TourAgencyClient.DTOs;
using TourAgencyClient.Services;

namespace TourAgencyClient.Services
{
    public class TourService : BaseService
    {
        private readonly PartnerService _partnerService;
        public TourService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, PartnerService partnerService)
            : base(httpClientFactory, httpContextAccessor)
        {
            _partnerService = partnerService;
        }

        public async Task<List<TourRead>?> GetToursAsync(TourSearchQuery query)
        {
            var url = "/api/tours";

            var queryParams = new Dictionary<string, string>();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Location))
                {
                    queryParams.Add("location", query.Location);
                }
                if (query.StartDate.HasValue)
                {
                    queryParams.Add("startDate", query.StartDate.Value.ToString("yyyy-MM-dd"));
                }
            }

            if (queryParams.Any())
            {
                url = QueryHelpers.AddQueryString(url, queryParams);
            }

            var result = await SendAsync<List<TourRead>>("TourApi", HttpMethod.Get, url);

            return result ?? new List<TourRead>();
        }

        public async Task<TourRead?> GetTourByIdAsync(int tourId)
        {
            var tourDetails = await SendAsync<TourRead>("TourApi", HttpMethod.Get, $"/api/tours/{tourId}");

            if (tourDetails == null) return null;

            if (tourDetails.PartnerIds != null && tourDetails.PartnerIds.Any())
            {
                tourDetails.PartnerNames = await _partnerService.GetPartnerNamesByIdsAsync(tourDetails.PartnerIds);
            }

            return tourDetails;
        }
    }
}