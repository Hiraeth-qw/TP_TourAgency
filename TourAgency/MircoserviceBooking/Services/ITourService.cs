using MicroserviceBooking.DTOs;

namespace MicroserviceBooking.Services
{
    public interface ITourService
    {
        Task<ReadTour?> GetTourAsync(int id);
        Task<bool> ReleaseSeatAsync(int id, int numberOfSeats, string token);
        Task<bool> ReserveSeatAsync(int id, int numberOfSeats, string token);
    }
}