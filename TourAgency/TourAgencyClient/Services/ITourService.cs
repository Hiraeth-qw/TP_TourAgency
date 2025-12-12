using TourAgencyClient.DTOs;

namespace TourAgencyClient.Services
{
    public interface ITourService : IBaseService
    {
        Task<ResponseDto<object>> CreateTourAsync(TourCreateUpdate dto);
        Task<ResponseDto<object>> DeleteTourAsync(int id);
        Task<TourRead?> GetTourByIdAsync(int tourId);
        Task<List<TourRead>?> GetToursAsync(TourSearchQuery query);
        Task<ResponseDto<object>> UpdateTourAsync(int id, TourCreateUpdate dto);
    }
}