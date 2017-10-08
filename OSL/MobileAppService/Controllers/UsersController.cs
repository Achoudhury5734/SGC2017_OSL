using System.Linq;
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

        // GET: api/values
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveAdmin(user)) {
                return new UnauthorizedResult();
            }

            return Ok(userRepository.Get());
        }

        // GET api/values/5
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveAdmin(user)) {
                return new UnauthorizedResult();
            }

            var u = userRepository.GetById(id);

            if (u != null) {
                return Ok(u);
            } else {
                return new NotFoundResult();
            }
        }

        // POST api/values
        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody]User value)
        {
            var oid = HttpContext.User.Claims.First(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            var email = HttpContext.User.Claims.First(c => c.Type == "emails")?.Value;
            var isNew = HttpContext.User.Claims.First(c => c.Type == "newUser")?.Value;

            if (isNew != "true") {
                return new BadRequestResult();
            }

            value.Oid = oid;
            value.Email = email;

            return Ok(userRepository.Create(value));
        }

        // PUT api/values/5
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]User value)
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveAdmin(user)) {
                return new UnauthorizedResult();
            }

            var u = userRepository.GetById(id);

            if (u != null)
            {
                return Ok(userRepository.UpdateUser(id, value));
            }
            else
            {
                return new NotFoundResult();
            }
        }

        // DELETE api/values/5
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveAdmin(user)) {
                return new UnauthorizedResult();
            }

            var u = userRepository.GetById(id);

            if (u != null)
            {
                userRepository.DeleteById(id);
                return StatusCode(204);
            }
            else
            {
                return new NotFoundResult();
            }
        }
    }
}
