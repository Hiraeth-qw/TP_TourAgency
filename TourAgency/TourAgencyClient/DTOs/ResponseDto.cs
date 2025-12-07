namespace TourAgencyClient.DTOs
{
    public class ResponseDto<T>
    {
        public bool IsSuccess { get; set; } = true;
        public T? Result { get; set; }
        public string? ErrorMessage { get; set; }
        public int StatusCode { get; set; }
    }
}
