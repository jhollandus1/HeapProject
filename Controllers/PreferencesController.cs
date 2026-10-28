using HeapProject.Identity;
using HeapProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace HeapProject.Controllers
{
    [Authorize]
    public class PreferencesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RequestLocalizationOptions _requestLocalizationOptions;
        private readonly IStringLocalizer<SharedResource> _stringLocalizer;


        public PreferencesController(UserManager<ApplicationUser> userManager, IOptions<RequestLocalizationOptions> requestLocalizationOptions, IStringLocalizer<SharedResource> stringLocalizer)
        {
            _userManager = userManager;
            _requestLocalizationOptions = requestLocalizationOptions.Value;
            _stringLocalizer = stringLocalizer;
        }
               
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound(_stringLocalizer["UsernameNotFound"] + " " + User.Identity?.Name);
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
                return NotFound(_stringLocalizer["UsernameNotFound"] + " " + User.Identity?.Name);
            }
            
            var cultureItems = _requestLocalizationOptions.SupportedUICultures
                                                        .Select(c => new SelectListItem { Text = c.DisplayName, Value = c.Name })
                                                        .ToList();

            var timeZoneList = TimeZoneInfo.GetSystemTimeZones()
                                            .Select(tz => new SelectListItem { Text = tz.DisplayName, Value = tz.Id })
                                            .ToList();

            if (!ModelState.IsValid)
            {   
                // this is needed to repopulate the lists           
                model.PreferredLocaleList = cultureItems;
                model.TimeZoneList = timeZoneList;

                ViewData["ErrorMessage"] = _stringLocalizer["FailedToUpdateUserSettings"].Value;
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
                    ModelState.AddModelError(nameof(model.SelectedPreferredLocaleId), _stringLocalizer["InvalidLocaleSelected"].Value);
                    condition1Culture = false;
                }
                //model.PreferredLocaleList = cultureItems;
                //model.TimeZoneList = timeZoneList;
            }

            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(model.SelectedTimeZone);
                if (timeZone != null)
                {
                    user.PreferredTimeZone = timeZone.Id;
                }
                else
                {
                    ModelState.AddModelError(nameof(model.SelectedTimeZone), _stringLocalizer["InvalidTimezoneSelected"].Value);
                    condition2TimeZone = false;
                }
            }
            catch (TimeZoneNotFoundException)
            {
                ModelState.AddModelError(nameof(model.SelectedTimeZone), _stringLocalizer["InvalidTimezoneSelected"].Value);
                condition2TimeZone = false;
                // we should not try to update the user on this exception - maybe reintroduce conditions 1 and 2
                
                //model.PreferredLocaleList = cultureItems;
                //model.TimeZoneList = timeZoneList;                
            }

            try
            {
                if(condition1Culture && condition2TimeZone)
                {
                    var result = await _userManager.UpdateAsync(user);
                    if(!result.Succeeded)
                    {
                        var errors = result.Errors.Select(e => e.Description);      // not internationalized
                        foreach (var error in errors)
                        {
                            ModelState.AddModelError(string.Empty, error);
                        }

                        // this is needed to repopulate the lists - TODO: ensure place not lost - probably the same due to postback selected attribute being saved           
                        model.PreferredLocaleList = cultureItems;
                        model.TimeZoneList = timeZoneList;

                        // return to here
                        ViewData["ErrorMessage"] = _stringLocalizer["FailedToUpdateUserSettings"].Value;
                        return View(model);
                    }

                    // force the new locale for the success message
                    var newCultureInfo = new CultureInfo(user.PreferredLocale);
                    CultureInfo.CurrentCulture = newCultureInfo;
                    CultureInfo.CurrentUICulture = newCultureInfo;

                    TempData["SuccessMessage"] = _stringLocalizer["SuccessfullyUpdatedUserSettings"].Value; 
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
                
                ViewData["ErrorMessage"] = _stringLocalizer["FailedToUpdateUserSettings"].Value;
                return View(model);
            }
        }
    }
}
