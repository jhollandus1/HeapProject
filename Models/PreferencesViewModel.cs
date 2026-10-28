using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HeapProject.Models
{
    public class PreferencesViewModel
    {
        [Display(Name = "PreferredLocale")]
        public string SelectedPreferredLocaleId { get; set; }
        // without the new List<SelectListItem>(), the TimeZoneList is null when the form is posted back, which causes the ModelState to be invalid

        public IEnumerable<SelectListItem> PreferredLocaleList { get; set; } = new List<SelectListItem>();


        [Display(Name = "PreferredTimeZone")]
        public string SelectedTimeZone { get; set; }
        // without the new List<SelectListItem>(), the TimeZoneList is null when the form is posted back, which causes the ModelState to be invalid
        public IEnumerable<SelectListItem> TimeZoneList { get; set; } = new List<SelectListItem>();
    }
}
