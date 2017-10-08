using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace OSL.MobileAppService.Controllers
{
    [Route("/")]
    public class AuthController : Controller
    {
        private string adminUser;
        private string adminPass;

        public AuthController(IConfigurationRoot configuration) {
            adminUser = configuration["Authentication:Admin:User"];
            adminPass = configuration["Authentication:Admin:Pass"];
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string user, string password)
        {
            if (user != adminUser || password != adminPass) {
                return new UnauthorizedResult();
            }

            var principal = new ClaimsPrincipal();

            await HttpContext.SignInAsync("MyCookieAuthenticationScheme", principal);

            return View("Login");
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuthenticationScheme");

            return RedirectToAction("Login");
        }

        [HttpGet("forbidden")]
        public IActionResult Forbidden()
        {
            return View("Forbidden");
        }
    }
}
