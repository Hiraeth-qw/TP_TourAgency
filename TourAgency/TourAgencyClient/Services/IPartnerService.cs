
namespace TourAgencyClient.Services
{
    public interface IPartnerService: IBaseService
    {
        Task<List<string>> GetPartnerNamesByIdsAsync(List<int> partnerIds);
    }
}