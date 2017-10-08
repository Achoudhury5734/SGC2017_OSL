using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
        public IActionResult Login(string ReturnUrl = "")
        {
            ViewData["ReturnUrl"] = ReturnUrl;

            return View("Login");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, string password, string ReturnUrl = "")
        {
            if (username != adminUser || password != adminPass) {
                return new UnauthorizedResult();
            }

            var claims = new[] {
                new Claim("name", "Admin"),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

            await HttpContext.SignInAsync("CookieScheme", principal, new AuthenticationProperties
            {
                IsPersistent = true,
            });

            if (!String.IsNullOrWhiteSpace(ReturnUrl)) {
                return Redirect(ReturnUrl);
            }

            return Redirect("/users");
        }

        [Authorize(AuthenticationSchemes = "CookieScheme")]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieScheme");

            return RedirectToAction("Login");
        }
    }
}
