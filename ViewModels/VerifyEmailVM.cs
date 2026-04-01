using System.ComponentModel.DataAnnotations;

namespace FanaCRM.ViewModels
{
    public class VerifyEmailVM
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }

    }
}