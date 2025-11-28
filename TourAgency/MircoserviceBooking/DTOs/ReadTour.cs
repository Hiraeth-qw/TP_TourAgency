namespace MicroserviceBooking.DTOs
{
    public class ReadTour
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int AvailableSeats { get; set; }
        public DateTime StartDate { get; set; }
        public List<int> PartnerIds { get; set; } = new();
    }
}
