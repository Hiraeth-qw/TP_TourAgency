using System.Text.Json.Serialization;

namespace TourAgencyClient.DTOs
{
    public class JsonPatchOperation
    {
        [JsonPropertyName("op")]
        public string Op { get; set; } = "replace";

        [JsonPropertyName("path")]
        public string Path { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public object? Value { get; set; }
    }
}
