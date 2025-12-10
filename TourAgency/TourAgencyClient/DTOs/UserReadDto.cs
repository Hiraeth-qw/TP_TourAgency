using System.ComponentModel.DataAnnotations;

namespace TourAgencyClient.DTOs
{
    public class UserReadDto
    {
        public string Id { get; set; }
        public string Email { get; set; }

        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Display(Name = "Телефон")]
        public string PhoneNumber { get; set; }

        public IList<string> Roles { get; set; } = new List<string>();

        public string RoleString => string.Join(", ", Roles);
    }
}
