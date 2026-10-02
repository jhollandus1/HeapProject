using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using HeapProject.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Rendering;
using HeapProject.Models;

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
               
        public IActionResult Index()
        {
            var currentCulture = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.UICulture.Name;
            var cultureItems = _requestLocalizationOptions.SupportedUICultures
                                                        .Select(c => new SelectListItem { Value = c.Name, Text = c.DisplayName, Selected = c.Name == currentCulture })
                                                        .ToList();

            PreferencesViewModel model = new PreferencesViewModel();
            model.PreferredLocaleList = cultureItems;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetLocale(PreferencesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            
            var user = await _userManager.GetUserAsync(User);
            foreach(var culture in _requestLocalizationOptions.SupportedUICultures)
            {
                if(user != null && culture.Name == model.SelectedPreferredLocaleId)
                {
                    user.PreferredLocale = culture.Name;
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

                    break;    
                }
            }

            // redirect if culture successfully set or if there is no match (user trying to spoof)
            return LocalRedirect("/Preferences");
        }
    }
}
