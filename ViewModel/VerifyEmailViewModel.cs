using System.ComponentModel.DataAnnotations;

namespace RoleBasedAuthentication.ViewModel
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "Email is reqiured")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
