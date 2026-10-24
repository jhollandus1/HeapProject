using HeapProject.Identity;
using HeapProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace HeapProject.Controllers
{
    [Authorize]
    public class PreferencesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RequestLocalizationOptions _requestLocalizationOptions;

        public PreferencesController(UserManager<ApplicationUser> userManager, IOptions<RequestLocalizationOptions> requestLocalizationOptions)
        {
            _userManager = userManager;
            _requestLocalizationOptions = requestLocalizationOptions.Value;
        }
               
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound("Username not found: " + User.Identity?.Name);
            }
            
            var cultureItems = _requestLocalizationOptions.SupportedUICultures
                                                    .Select(c => new SelectListItem { Value = c.Name, Text = c.DisplayName, Selected = c.Name == user.PreferredLocale })
                                                    .ToList(); 
            var timeZoneList = TimeZoneInfo.GetSystemTimeZones()
                                            .Select(tz => new SelectListItem { Text = tz.DisplayName, Value = tz.Id, Selected = tz.Id == user.PreferredTimeZone })
                                            .ToList();

            PreferencesViewModel model = new PreferencesViewModel()
            {
                PreferredLocaleList = cultureItems,
                TimeZoneList = timeZoneList
            };

            return View(model);           
        }

        [HttpPost]
        public async Task<IActionResult> Index(PreferencesViewModel model)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Username not found: " + User.Identity?.Name);
            }
            
            var cultureItems = _requestLocalizationOptions.SupportedUICultures
                                                        .Select(c => new SelectListItem { Value = c.Name, Text = c.DisplayName })
                                                        .ToList();

            var timeZoneList = TimeZoneInfo.GetSystemTimeZones()
                                            .Select(tz => new SelectListItem { Text = tz.DisplayName, Value = tz.Id })
                                            .ToList();


            if (!ModelState.IsValid)
            {   
                // this is needed to repopulate the lists           
                model.PreferredLocaleList = cultureItems;
                model.TimeZoneList = timeZoneList;   
                
                TempData["ErrorMessage"] = "Failed to update user settings.";

                return View(model);
            }
            
            bool condition1Culture = true;
            bool condition2TimeZone = true;

            CultureInfo culture = CultureInfo.CurrentUICulture;
            for(int i = 0; i <  _requestLocalizationOptions.SupportedUICultures.Count; i++)
            {
                culture = _requestLocalizationOptions.SupportedUICultures[i];
                if(user != null && culture.Name == model.SelectedPreferredLocaleId)
                {
                    user.PreferredLocale = culture.Name;            
                    break;    
                }
                if (i == _requestLocalizationOptions.SupportedUICultures.Count - 1)
                {
                    ModelState.AddModelError(nameof(model.SelectedPreferredLocaleId), "Invalid locale selected.");
                    condition1Culture = false;
                }
                //model.PreferredLocaleList = cultureItems;
                //model.TimeZoneList = timeZoneList;
            }

            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(model.SelectedTimeZone);
                if (user != null && timeZone != null)
                {
                    user.PreferredTimeZone = timeZone.Id;
                }
                else
                {
                    ModelState.AddModelError(nameof(model.SelectedTimeZone), "Invalid timezone selected.");
                    condition2TimeZone = false;
                }
            }
            catch (TimeZoneNotFoundException)
            {
                ModelState.AddModelError(nameof(model.SelectedTimeZone), "Invalid timezone selected.");
                condition2TimeZone = false;
                // we should not try to update the user on this exception - maybe reintroduce conditions 1 and 2
                
                //model.PreferredLocaleList = cultureItems;
                //model.TimeZoneList = timeZoneList;                
            }

            try
            {
                if (user != null && condition1Culture && condition2TimeZone)
                {
                    var result = await _userManager.UpdateAsync(user);
                    if(!result.Succeeded)
                    {
                        // return to here
                    }
                    TempData["SuccessMessage"] = "Succesfully updated user settings.";  // TODO: internationalize
                    return RedirectToAction("Index", "Preferences");
                }
                else
                {
                    throw new Exception();
                }
                
            }
            catch (Exception)
            {                
                model.PreferredLocaleList = cultureItems;
                model.TimeZoneList = timeZoneList;
                
                TempData["ErrorMessage"] = "Failed to update user settings.";
                return View(model);
            }
        }
    }
}
