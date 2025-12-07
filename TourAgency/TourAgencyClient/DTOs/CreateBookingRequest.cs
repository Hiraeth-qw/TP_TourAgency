using System.ComponentModel.DataAnnotations;

namespace TourAgencyClient.DTOs
{
    public class CreateBookingRequest
    {
        public int TourId { get; set; }
        public int TouristsNumber { get; set; }
        public string? ClientUserId { get; set; }
        public int CartItemId { get; set; }
    }
}
