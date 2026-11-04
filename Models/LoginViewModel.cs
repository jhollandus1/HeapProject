using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HeapProject.Models
{
    public class LoginViewModel
    {
        [EmailAddress(ErrorMessage = "PleaseEnterAValidEmailAddress")]        
        [Required(ErrorMessage = "FieldRequired")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "FieldRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "RememberMe")]
        public bool RememberMe { get; set; }
    }
}
