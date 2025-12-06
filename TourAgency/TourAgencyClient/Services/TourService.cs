using TourAgencyClient.Services;
using TourAgencyClient.DTOs;

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

        public async Task<List<TourRead>?> GetAllToursAsync()
        {
            var result = await SendAsync<List<TourRead>>("TourApi", HttpMethod.Get, "/api/tours");

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