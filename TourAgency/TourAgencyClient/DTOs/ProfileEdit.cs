using System.ComponentModel.DataAnnotations;

namespace TourAgencyClient.DTOs
{
    public class ProfileEdit
    {
        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Поле 'Имя' обязательно.")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = "Поле 'Фамилия' обязательно.")]
        public string LastName { get; set; }

        [Display(Name = "Телефон")]
        [Phone(ErrorMessage = "Некорректный формат телефона.")]
        public string? PhoneNumber { get; set; }
    }
}
