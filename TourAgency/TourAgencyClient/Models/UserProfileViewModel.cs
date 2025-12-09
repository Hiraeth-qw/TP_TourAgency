using System.ComponentModel.DataAnnotations;

namespace TourAgencyClient.Models
{
    public class UserProfileViewModel
    {
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Телефон")]
        public string PhoneNumber { get; set; }

        public List<CountryStatsViewModel> CountryStats { get; set; } = new List<CountryStatsViewModel>();
    }
}
