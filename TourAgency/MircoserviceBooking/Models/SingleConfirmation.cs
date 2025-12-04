namespace MicroserviceBooking.Models
{
    public class SingleConfirmation
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public long PartnerId { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime ConfirmationDate { get; set; } = DateTime.UtcNow;
    }
}