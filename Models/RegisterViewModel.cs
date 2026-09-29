using System.ComponentModel.DataAnnotations;

namespace HeapProject.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        [Display(Name = "First name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        [Display(Name = "Last name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 100 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long and at most 100 characters long")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        // TODO1: needs annotations, and checked
        [Display(Name = "UserName")]
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; } = string.Empty;
    }
}
