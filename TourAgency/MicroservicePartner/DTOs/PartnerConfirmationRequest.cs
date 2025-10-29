namespace MicroservicePartner.DTOs
{
    public class PartnerConfirmationRequest
    {
        public long PartnerId { get; set; }
        public long BookingId { get; set; }
        public DateTime ServiceStartDate { get; set; }
        public string Details { get; set; }
        public long TourId { get; set; }
    }
}
