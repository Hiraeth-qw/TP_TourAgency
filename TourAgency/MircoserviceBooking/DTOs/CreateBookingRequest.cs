using System.ComponentModel.DataAnnotations;

namespace MicroserviceBooking.DTOs
{
    public class CreateBookingRequest
    {
        [Required]
        public int TourId { get; set; }
        [Required]
        [Range(1, 10)]
        public int TouristsNumber { get; set; }
        public string? ClientUserId { get; set; }
    }
}
