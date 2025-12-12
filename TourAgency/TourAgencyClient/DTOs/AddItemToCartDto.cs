using System.ComponentModel.DataAnnotations;

namespace TourAgencyClient.DTOs
{
    public class AddItemToCartDto
    {
        public int TourId { get; set; }
        public int TouristsNumber { get; set; } = 1;
    }
}
