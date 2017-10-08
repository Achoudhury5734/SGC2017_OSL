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
        private readonly UserRepository Users;

        public UsersController(UserRepository userRepository)
        {
            Users = userRepository;
        }

        // GET: api/values
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            var user = Users.GetUserFromPrincipal(HttpContext.User);
            if (user != null && (!user.Admin || user.Status != UserStatus.Active)) {
                return new UnauthorizedResult();
            }

            return Ok(Users.Get());
        }

        // GET api/values/5
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = Users.GetUserFromPrincipal(HttpContext.User);
            if (user != null && (!user.Admin || user.Status != UserStatus.Active)) {
                return new UnauthorizedResult();
            }

            var u = Users.GetById(id);

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

            return Ok(Users.Create(value));
        }

        // PUT api/values/5
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]User value)
        {
            var user = Users.GetUserFromPrincipal(HttpContext.User);
            if (user != null && (!user.Admin || user.Status != UserStatus.Active))
            {
                return new UnauthorizedResult();
            }

            var u = Users.GetById(id);

            if (u != null)
            {
                return Ok(Users.UpdateUser(id, value));
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
            var user = Users.GetUserFromPrincipal(HttpContext.User);
            if (user != null && (!user.Admin || user.Status != UserStatus.Active))
            {
                return new UnauthorizedResult();
            }

            var u = Users.GetById(id);

            if (u != null)
            {
                Users.DeleteById(id);
                return StatusCode(204);
            }
            else
            {
                return new NotFoundResult();
            }
        }
    }
}
