using System.ComponentModel.DataAnnotations;

namespace Eventra.Models.ViewModels
{
    public class EditProfileViewModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
    }
}