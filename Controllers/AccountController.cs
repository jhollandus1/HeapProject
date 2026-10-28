using HeapProject.Identity;
using HeapProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace HeapProject.Controllers
{
    public class AccountController : SharedController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RequestLocalizationOptions _requestLocalizationOptions;
        private readonly IStringLocalizer<SharedResource> _stringLocalizer;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, 
                                    IOptions<RequestLocalizationOptions> requestLocalizationOptions, IStringLocalizer<SharedResource> stringLocalizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _requestLocalizationOptions = requestLocalizationOptions.Value;
            _stringLocalizer = stringLocalizer;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpGet]
        public IActionResult Index() => View();

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(email);
                
                if(user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: false);

                    // different from noted, need authorize attribute on this controller eventually
                    if (result.Succeeded)
                    {                      
                        return RedirectToAction("Index", "HeapHistoryItems");
                    }
                    else
                    {
                        //ModelState.AddModelError(string.Empty, _stringLocalizer["InvalidLoginAttempt"].Value);
                        ViewData["ErrorMessage"] = _stringLocalizer["InvalidLoginAttempt"].Value;   // tested
                    }                    
                }
                else
                {
                    //ModelState.AddModelError(string.Empty, _stringLocalizer["InvalidLoginAttempt"].Value);       // could be more specific, but don't give away too much info (security issue)
                    ViewData["ErrorMessage"] = _stringLocalizer["InvalidLoginAttempt"].Value;       // tested
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public async Task<IActionResult> Register()
        {
            RegisterViewModel model = new RegisterViewModel();
            
            var cultureItems = new List<SelectListItem>();
            cultureItems.Add(new SelectListItem { Text = _stringLocalizer["SelectAnItem"].Value, Value = string.Empty, Selected = true, Disabled = true });
            cultureItems.AddRange(_requestLocalizationOptions.SupportedUICultures
                                            .Select(c => new SelectListItem { Text = c.DisplayName, Value = c.Name })
                                            .ToList());
            
            model.PreferredLocaleList = cultureItems;

            var timeZoneList = new List<SelectListItem>();
            timeZoneList.Add(new SelectListItem { Text = _stringLocalizer["SelectAnItem"].Value, Value = string.Empty, Selected = true, Disabled = true });
            timeZoneList.AddRange(TimeZoneInfo.GetSystemTimeZones()
                                            .Select(tz => new SelectListItem { Text = tz.DisplayName, Value = tz.Id })
                                            .ToList());
            
            model.TimeZoneList = timeZoneList;
            //test
            string uiCulture = SetViewModelDropDownBasedOnCookie(model);
            model.PreferredLocaleList.FirstOrDefault(x => x.Value == uiCulture)?.Selected = true;

            if (((string)TempData["language"]) != null)
            {
                string culture;
                if (((string)TempData["language"]) == "en")
                {
                    culture = "en";
                }
                else if (((string)TempData["language"]) == "es")
                {
                    culture = "es";
                }
                else if (((string)TempData["language"]) == "fr")
                {
                    culture = "fr";
                }              
                else  // default to English
                {
                    culture = "en";
                }

                Response.Cookies.Append(
                         CookieRequestCultureProvider.DefaultCookieName,
                         CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(new CultureInfo(culture))),
                         new CookieOptions
                         {
                             Expires = DateTimeOffset.UtcNow.AddYears(1),
                             HttpOnly = true,
                             Secure = true,
                             SameSite = SameSiteMode.Strict
                         }
                 );

                TempData["language"] = null;
                return RedirectToAction("Register", "Account");
            }
            //if (((string)TempData["cookieLanguage"]) != null)
            //{
            //    model.LanguageVM.SelectedPreferredLanguage = (string)TempData["cookieLanguage"];
            //    // this line should not be necessary
            //    //model.LanguageVM.LanguageList.FirstOrDefault(x => x.Value == model.LanguageVM.SelectedPreferredLanguage)?.Selected = true;                
            //}

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> SelectPreferredLanguage([Bind("LanguageVM")] RegisterViewModel registerViewModel)
        {
            // have to remove these items for model validation to succeed, look to repopulate lists elsewhere, need to synchronize code from register over, check in something
            ModelState.Remove(nameof(registerViewModel.Email));
            ModelState.Remove(nameof(registerViewModel.LastName));
            ModelState.Remove(nameof(registerViewModel.Password));
            ModelState.Remove(nameof(registerViewModel.UserName));
            ModelState.Remove(nameof(registerViewModel.FirstName));
            ModelState.Remove(nameof(registerViewModel.SelectedTimeZone));
            ModelState.Remove(nameof(registerViewModel.SelectedPreferredLocaleId));
            if (!ModelState.IsValid)
            {
                SetViewModelDropDownBasedOnCookie(registerViewModel);                
                return View("Register", registerViewModel); 
            }

            TempData["language"] = registerViewModel.LanguageVM.SelectedPreferredLanguage;
            return RedirectToAction("Register");
        }


        // have 

        [ValidateAntiForgeryToken]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(RegisterViewModel registerViewModel)
        {
            var cultureItems = new List<SelectListItem>();
            cultureItems.Add(new SelectListItem { Text = _stringLocalizer["SelectAnItem"].Value, Value = string.Empty, Selected = true, Disabled = true });
            cultureItems.AddRange(_requestLocalizationOptions.SupportedUICultures
                                            .Select(c => new SelectListItem { Text = c.DisplayName, Value = c.Name })
                                            .ToList());

            var timeZoneList = new List<SelectListItem>();
            timeZoneList.Add(new SelectListItem { Text = _stringLocalizer["SelectAnItem"].Value, Value = string.Empty, Selected = true, Disabled = true });
            timeZoneList.AddRange(TimeZoneInfo.GetSystemTimeZones()
                                            .Select(tz => new SelectListItem { Text = tz.DisplayName, Value = tz.Id })
                                            .ToList());

            ModelState.Remove("LanguageVM.SelectedPreferredLanguage");
            if(!ModelState.IsValid)
            {
                registerViewModel.PreferredLocaleList = cultureItems;
                registerViewModel.TimeZoneList = timeZoneList;

                SetViewModelDropDownBasedOnCookie(registerViewModel);
                // wrong - user could have overridden - in which case we want to leave as is - registerViewModel.PreferredLocaleList.FirstOrDefault(x => x.Value == uiCulture)?.Selected = true;

                ViewData["ErrorMessage"] = _stringLocalizer["FailedToRegisterNewUser"].Value;  //  "Failed to register a new user";
                return View("Register", registerViewModel);
            }

            ApplicationUser applicationUser = new ApplicationUser
             {
                 FirstName1 = registerViewModel.FirstName,
                 LastName1 = registerViewModel.LastName,
                 Email = registerViewModel.Email,
                 UserName = registerViewModel.UserName
             };

            // check the locale and timezone values against items in the lists
            bool condition1Culture = true;
            bool condition2TimeZone = true;

            CultureInfo culture = CultureInfo.CurrentUICulture;
            for (int i = 0; i < _requestLocalizationOptions.SupportedUICultures.Count; i++)
            {
                culture = _requestLocalizationOptions.SupportedUICultures[i];
                if (applicationUser != null && culture.Name == registerViewModel.SelectedPreferredLocaleId)
                {
                    applicationUser.PreferredLocale = culture.Name;
                    break;
                }
                if (i == _requestLocalizationOptions.SupportedUICultures.Count - 1)
                {
                    ModelState.AddModelError(nameof(registerViewModel.SelectedPreferredLocaleId), _stringLocalizer["InvalidLocaleSelected"].Value);
                    condition1Culture = false;
                }
                //model.PreferredLocaleList = cultureItems;
                //model.TimeZoneList = timeZoneList;
            }

            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(registerViewModel.SelectedTimeZone);
                if (timeZone != null)
                {
                    applicationUser.PreferredTimeZone = timeZone.Id;
                }
                else
                {
                    ModelState.AddModelError(nameof(registerViewModel.SelectedTimeZone), _stringLocalizer["InvalidTimezoneSelected"].Value);
                    condition2TimeZone = false;
                }
            }
            catch (TimeZoneNotFoundException)
            {
                ModelState.AddModelError(nameof(registerViewModel.SelectedTimeZone), _stringLocalizer["InvalidTimezoneSelected"].Value);
                condition2TimeZone = false;
            }

            try
            {
                if (condition1Culture && condition2TimeZone)
                {
                    var result = await _userManager.CreateAsync(applicationUser, registerViewModel.Password);
                                     
                    // also remove cookies - ugly with multiple users of different locales using same program - not impossible
                    if(!result.Succeeded)
                    {
                        var errors = result.Errors.Select(e => e.Description);      // not internationalized
                        foreach (var error in errors)
                        {
                            ModelState.AddModelError(string.Empty, error);
                        }

                        registerViewModel.PreferredLocaleList = cultureItems;
                        registerViewModel.TimeZoneList = timeZoneList;
                        
                        SetViewModelDropDownBasedOnCookie(registerViewModel);
                        ViewData["ErrorMessage"] = _stringLocalizer["FailedToRegisterNewUser"].Value;  //  "Failed to register a new user";
                        return View("Register", registerViewModel);     // BadRequest(new { Errors = errors });
                    }

                    // force the new locale for the success message
                    var newCultureInfo = new CultureInfo(applicationUser.PreferredLocale);
                    CultureInfo.CurrentCulture = newCultureInfo;
                    CultureInfo.CurrentUICulture = newCultureInfo;

                    TempData["SuccessMessage"] = _stringLocalizer["SuccessfullyRegisteredNewUser"].Value;
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    throw new Exception();
                }

            }
            catch (Exception)
            {
                registerViewModel.PreferredLocaleList = cultureItems;
                registerViewModel.TimeZoneList = timeZoneList;
                SetViewModelDropDownBasedOnCookie(registerViewModel);

                ViewData["ErrorMessage"] = _stringLocalizer["FailedToRegisterNewUser"].Value; 
                return View("Register", registerViewModel);
            }
        }

    }            
            
}
