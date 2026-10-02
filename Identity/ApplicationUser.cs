using Microsoft.AspNetCore.Identity;

namespace HeapProject.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName1 { get; set; }
        public string LastName1 { get; set; }

        // stores strings like "en-US", "es-ES", or "fr-FR"
        public string PreferredLocale { get; set; } = "en-US";
    }
}
