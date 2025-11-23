using System.ComponentModel.DataAnnotations;

namespace MicroserviceUser.DTOs
{
    public class AssignRole
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string RoleName { get; set; }
    }
}
