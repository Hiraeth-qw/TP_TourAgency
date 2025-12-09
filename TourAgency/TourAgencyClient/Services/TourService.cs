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

            var resp = await SendAsync<List<TourRead>>("TourApi", HttpMethod.Get, url);
            var result = resp.Result;

            return result ?? new List<TourRead>();
        }

        public async Task<TourRead?> GetTourByIdAsync(int tourId)
        {
            var resp = await SendAsync<TourRead>("TourApi", HttpMethod.Get, $"/api/tours/{tourId}");
            var tourDetails = resp.Result;

            if (tourDetails == null) return null;

            if (tourDetails.PartnerIds != null && tourDetails.PartnerIds.Any())
            {
                tourDetails.PartnerNames = await _partnerService.GetPartnerNamesByIdsAsync(tourDetails.PartnerIds);
            }

            return tourDetails;
        }
        public async Task<ResponseDto<object>> CreateTourAsync(TourCreateUpdate dto)
        {
            return await SendAsync<object>("TourApi", HttpMethod.Post, "/api/tours", dto);
        }

        public async Task<ResponseDto<object>> UpdateTourAsync(int id, TourCreateUpdate dto)
        {
            return await SendAsync<object>("TourApi", HttpMethod.Put, $"/api/tours/{id}", dto);
        }

        public async Task<ResponseDto<object>> DeleteTourAsync(int id)
        {
            return await SendAsync<object>("TourApi", HttpMethod.Delete, $"/api/tours/{id}");
        }
    }
}