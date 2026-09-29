using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HeapProject.Identity;
using HeapProject.Models;

namespace HeapProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;            
        }

        [HttpGet]
        public IActionResult Login() => View();

        public IActionResult Index()
        {
            return View();
        }

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

                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
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
        public IActionResult Register() => View();


        [ValidateAntiForgeryToken]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromForm] RegisterViewModel registerViewModel)
        {
            if(!ModelState.IsValid)
            {                
                return View("Register");
            }

            var applicationUser = new ApplicationUser
            {
                FirstName1 = registerViewModel.FirstName,
                LastName1 = registerViewModel.LastName,
                Email = registerViewModel.Email,
                UserName = registerViewModel.UserName
            };

            var result = await _userManager.CreateAsync(applicationUser, registerViewModel.Password);

            if(!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View("Register");     // BadRequest(new { Errors = errors });
            }

            return View("Login");  // StatusCode(201);  // Created
        }

    }            
            
}
