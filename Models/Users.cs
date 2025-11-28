using Microsoft.AspNetCore.Identity;

namespace RoleBasedAuthentication.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }
    }
}
