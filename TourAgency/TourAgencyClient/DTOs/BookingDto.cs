namespace TourAgencyClient.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public DateTime BookingDate { get; set; }
        public int NumberOfSeats { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }
}
