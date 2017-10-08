using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSL.MobileAppService.Models;
using OSL.MobileAppService.Services;

namespace OSL.MobileAppService.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly UserRepository userRepository;

        public UsersController(UserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        // GET api/users/me
        [Authorize]
        [HttpGet("me")]
        public IActionResult Get()
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);

            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return new NotFoundResult();
            }
        }

        // POST api/users
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]User value)
        {
            var oid = HttpContext.User.Claims.First(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            var email = HttpContext.User.Claims.First(c => c.Type == "emails")?.Value;
            var isNew = HttpContext.User.Claims.First(c => c.Type == "newUser")?.Value;

            if (isNew != "true")
            {
                return new BadRequestResult();
            }

            value.Oid = oid;
            value.Email = email;

            return Ok(await userRepository.Create(value));
        }

        // PUT api/users/me
        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> Update([FromBody]User value)
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);

            if (user != null)
            {
                value.Verified = user.Verified;
                value.Admin = user.Admin;
                value.Status = user.Status;

                return Ok(await userRepository.UpdateUser(user.Id, value));
            }
            else
            {
                return new NotFoundResult();
            }
        }
    }
}