using Azure.Core;
using HeapProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HeapProject
{
    public class SharedController : Controller
    {
        public string SetViewModelDropDownBasedOnCookie(RegisterViewModel registerViewModel)
        {
            registerViewModel.LanguageVM.LanguageList = new List<SelectListItem>
            {
                new SelectListItem { Text = "English", Value = "en" },
                new SelectListItem { Text = "Español", Value = "es" },
                new SelectListItem { Text = "Français", Value = "fr" }
            };

            // on the initial load, read the cookie and set the language list accordingly
            string? cookie = Request.Cookies[".AspNetCore.Culture"];
            string uiCulture = cookie?.Split("|")[1]?.Split("=")[1] ?? "en";
            registerViewModel.LanguageVM.LanguageList.FirstOrDefault(x => x.Value == uiCulture)?.Selected = true;    // resave the selected locale

            return uiCulture;
        }

    }
}
