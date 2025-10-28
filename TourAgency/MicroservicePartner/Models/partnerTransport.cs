namespace MicroservicePartner.Models
{
    public enum TransportType { flight, bus, train, ship }
    public class partnerTransport : Partner
    {
        public TransportType TransportType { get; set; }
    }
}
