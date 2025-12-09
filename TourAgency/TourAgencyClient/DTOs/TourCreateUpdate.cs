using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TourAgencyClient.DTOs
{
    public class TourCreateUpdate
    {
        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(100)]
        [Display(Name = "Название тура")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Местоположение обязательно")]
        [Display(Name = "Место (Страна, Город)")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Описание обязательно")]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Дата начала")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Дата окончания")]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Количество мест")]
        public int AvailableSeats { get; set; }

        public List<int> PartnerIds { get; set; } = new();
    }
}