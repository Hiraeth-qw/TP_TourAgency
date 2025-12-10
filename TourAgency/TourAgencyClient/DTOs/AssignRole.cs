using System.ComponentModel.DataAnnotations;

namespace TourAgencyClient.DTOs
{
    public class AssignRole
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string RoleName { get; set; }
    }
}
