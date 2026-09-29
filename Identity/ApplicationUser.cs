using Microsoft.AspNetCore.Identity;

namespace HeapProject.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName1 { get; set; }
        public string LastName1 { get; set; }
    }
}
