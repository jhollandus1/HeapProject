using HeapProject.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace HeapProject.CultureProvider
{
    public class DatabaseCultureProvider : RequestCultureProvider
    {
        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null) return null;

            // Check if the user is logged in
            if(httpContext.User.Identity?.IsAuthenticated == true)
            {
                // Scoped resolution to get the UserManager
                var userManager = httpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                var user = await userManager.GetUserAsync(httpContext.User);

                if(user != null && !string.IsNullOrEmpty(user.PreferredLocale))
                {
                    return new ProviderCultureResult(user.PreferredLocale, user.PreferredLocale);
                }
            }

            // Return null if user is not logged in or has not selected a locale;
            // framework will fall back to Cookies, QueryString, or System Default
            return null;
        }
    }
}
