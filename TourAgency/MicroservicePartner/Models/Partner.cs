using System.Text.Json.Serialization;

namespace MicroservicePartner.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Type { Operator, Hotel, Transport }
    public class Partner
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ContactInfo { get; set; }
        public Type PartnerType { get; set; }
    }
}