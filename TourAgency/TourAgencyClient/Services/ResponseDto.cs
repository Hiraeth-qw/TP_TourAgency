namespace TourAgencyClient.Services
{
    public class ResponseDto<T>
    {
        public bool IsSuccess { get; set; } = true;
        public T? Result { get; set; }
        public string Message { get; set; } = "";
    }
}
