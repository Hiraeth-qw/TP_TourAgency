namespace TourAgencyClient.DTOs
{
    public class CartItemDto
    {
        public int CartItemId { get; set; }
        public int TourId { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public decimal Price{ get; set; }
        public DateTime StartDate { get; set; }
        public int NumberOfSeats { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
