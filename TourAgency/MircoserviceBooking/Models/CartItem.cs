namespace MicroserviceBooking.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int TourId { get; set; }
        public int NumberOfSeats { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.UtcNow.AddHours(3);
    }
}