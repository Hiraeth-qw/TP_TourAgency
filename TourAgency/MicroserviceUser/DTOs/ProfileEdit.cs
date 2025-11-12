using System.ComponentModel.DataAnnotations;

namespace MicroserviceUser.DTOs
{
    public class ProfileEdit
    {
        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }
    }
}
