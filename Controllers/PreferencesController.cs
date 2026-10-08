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
            // is this based on cookies or DB, DB should come first, should be able to remove cookie if user has a preference in DB, right?
            var currentCulture = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.UICulture.Name;
            var cultureItems = _requestLocalizationOptions.SupportedUICultures
                                                        .Select(c => new SelectListItem { Value = c.Name, Text = c.DisplayName, Selected = c.Name == currentCulture })
                                                        .ToList();

            PreferencesViewModel model = new PreferencesViewModel();
            model.PreferredLocaleList = cultureItems;

            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user != null) 
            {
                var timeZoneList = TimeZoneInfo.GetSystemTimeZones()
                                                .Select(tz => new SelectListItem { Text = tz.DisplayName, Value = tz.Id, Selected = tz.Id == user.PreferredTimeZone })
                                                .ToList();
                model.TimeZoneList = timeZoneList;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(PreferencesViewModel model)
        {
            var currentCulture = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.UICulture.Name;
            var cultureItems = _requestLocalizationOptions.SupportedUICultures
                                                        .Select(c => new SelectListItem { Value = c.Name, Text = c.DisplayName, Selected = c.Name == currentCulture })
                                                        .ToList();

            var timeZoneList = TimeZoneInfo.GetSystemTimeZones()
                                            .Select(tz => new SelectListItem { Text = tz.DisplayName, Value = tz.Id })
                                            .ToList();

            bool condition1Culture = true;
            bool condition2TimeZone = true;

            if (!ModelState.IsValid)
            {   
                // this is needed to repopulate the lists           
                model.PreferredLocaleList = cultureItems;
                model.TimeZoneList = timeZoneList;   
                
                TempData["ErrorMessage"] = "Failed to update user settings.";

                return View(model);
            }

            ApplicationUser? user = await _userManager.GetUserAsync(User);
            
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
                if (user != null && timeZone != null && condition1Culture)
                {
                    user.PreferredTimeZone = timeZone.Id;
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
                    await _userManager.UpdateAsync(user);
                    Response.Cookies.Append(
                            CookieRequestCultureProvider.DefaultCookieName,
                            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                            new CookieOptions
                            {
                                Expires = DateTimeOffset.UtcNow.AddYears(1),
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Strict
                            }
                    );
                    TempData["SuccessMessage"] = "Succesfully updated user settings.";
                    return LocalRedirect("/Preferences");
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
