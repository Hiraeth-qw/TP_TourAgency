namespace MicroserviceTour.Models
{
    public class Tour
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public int AvailableSeats { get; set; }
        public List<int> PartnerIds { get; set; } = new();
    }
}
