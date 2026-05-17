using System.ComponentModel.DataAnnotations;

namespace Eventra.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string EmailOrUsername { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}