using System.ComponentModel.DataAnnotations;

namespace Eventra.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Please enter your email or username.")]
        public string EmailOrUsername { get; set; } = string.Empty;
    }
}
