using System.ComponentModel.DataAnnotations;

namespace TourAgencyClient.Models
{
    public class BookingViewModel
    {
        public int Id { get; set; }
        public int TourId { get; set; }

        [Display(Name = "Тур")]
        public string TourTitle { get; set; }

        [Display(Name = "Дата отправления")]
        public DateTime TourStartDate { get; set; }

        [Display(Name = "Дата окончания")]
        public DateTime TourEndDate { get; set; }

        [Display(Name = "Дата бронирования")]
        public DateTime BookingDate { get; set; }

        [Display(Name = "Мест")]
        public int NumberOfSeats { get; set; }

        [Display(Name = "Сумма")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalAmount { get; set; }

        public string Status { get; set; }
    }
}
