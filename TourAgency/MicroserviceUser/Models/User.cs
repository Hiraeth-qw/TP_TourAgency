using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MicroserviceUser.Models
{
    public class User: IdentityUser
    {
        public string firstName {  get; set; }
        public string lastName { get; set; }
    }
}
