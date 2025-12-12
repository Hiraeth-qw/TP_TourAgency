using MicroserviceBooking.DTOs;

namespace MicroserviceBooking.Services
{
    public interface IPartnerService
    {
        Task<bool> ConfirmBookingAsync(PartnerConfirmationRequest request, string token);
    }
}