using Microsoft.AspNetCore.Identity;

namespace HeapProject.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName1 { get; set; }
        public string LastName1 { get; set; }

        // stores strings like "en-US", "es-ES", or "fr-FR"
        public string PreferredLocale { get; set; } = "en-US";

        // OS_COMPATIBILITY: Current implementation only supports Windows: additional code required for Linux
        public string PreferredTimeZone { get; set; } = TimeZoneInfo.Local.Id;  // OS that this application is on 
    }
}
