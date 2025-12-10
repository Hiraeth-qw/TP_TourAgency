using System.ComponentModel.DataAnnotations;

namespace TourAgencyClient.Models
{
    public class CartItemViewModel
    {
        public int CartItemId { get; set; }
        public int TourId { get; set; }

        [Display(Name = "Название")]
        public string Title { get; set; }

        [Display(Name = "Место")]
        public string Location { get; set; }

        [Display(Name = "Даты")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Цена за место")]
        public decimal Price { get; set; }

        [Display(Name = "Количество туристов")]
        public int TouristsNumber { get; set; }

        [Display(Name = "Итого")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Добавлено")]
        public DateTime AddedDate { get; set; }
    }
}