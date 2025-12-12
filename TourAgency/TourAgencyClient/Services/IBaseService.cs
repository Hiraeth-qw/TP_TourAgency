using TourAgencyClient.DTOs;

namespace TourAgencyClient.Services
{
    public interface IBaseService
    {
        Task<ResponseDto<T>> SendAsync<T>(string clientName, HttpMethod method, string url, object? data = null);
    }
}