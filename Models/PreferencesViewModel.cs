using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HeapProject.Models
{
    public class PreferencesViewModel
    {
        [Display(Name = "Preferred Locale")]
        public string SelectedPreferredLocaleId { get; set; }
        public List<SelectListItem> PreferredLocaleList { get; set; } = new List<SelectListItem>();
    }
}
