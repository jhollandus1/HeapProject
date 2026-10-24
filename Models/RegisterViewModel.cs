using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HeapProject.Models
{
    public class RegisterViewModel
    {
        public LanguageViewModel LanguageVM { get; set; } = new LanguageViewModel();

        [Required(ErrorMessage = "FieldRequired")]
        [StringLength(100, ErrorMessage = "FirstNameCannotExceed100")]                  
        [Display(Name = "FirstName")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "FieldRequired")]
        [StringLength(100, ErrorMessage = "LastNameCannotExceed100")]
        [Display(Name = "LastName")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "FieldRequired")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "EmailBetween5And100Characters")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "FieldRequired")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "PasswordBetween8And100Characters")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        // need required attribute here?
        //[Required(ErrorMessage = "FieldRequired")]        // this line is messing up the registration page - cannot change languages
        [Display(Name = "ConfirmPassword")]
        [Compare("Password", ErrorMessage = "PasswordsDoNotMatch")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "UserName")]
        [Required(ErrorMessage = "FieldRequired")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "UserNameBetween2And100Characters")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "PreferredLocale")]
        [Required(ErrorMessage = "FieldRequired")]  // tested internationalization on this field
        public string SelectedPreferredLocaleId { get; set; }
        public IEnumerable<SelectListItem> PreferredLocaleList { get; set; } = new List<SelectListItem>();

        [Display(Name = "PreferredTimeZone")]
        [Required(ErrorMessage = "FieldRequired")]
        public string SelectedTimeZone { get; set; }
        public IEnumerable<SelectListItem> TimeZoneList { get; set; } = new List<SelectListItem>();

        public class LanguageViewModel
        {
            [Display(Name = "Language")]
            public string SelectedPreferredLanguage { get; set; }
            public IEnumerable<SelectListItem> LanguageList { get; set; } = new List<SelectListItem>();
        }
    }
}
