using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSL.MobileAppService.Services;

namespace OSL.MobileAppService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly UserRepository Users;

        public UsersController(UserRepository userRepository)
        {
            Users = userRepository;
        }

        // GET: api/values
        [HttpGet]
        public IActionResult Get()
        {
            var user = Users.GetUserFromPrincipal(HttpContext.User);
            if (!user.Admin) {
                return new UnauthorizedResult();
            }

            return Ok(Users.Get());
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = Users.GetUserFromPrincipal(HttpContext.User);
            if (!user.Admin)
            {
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
        [HttpPost]
        public IActionResult Post([FromBody]string value)
        {
            var user = Users.GetUserFromPrincipal(HttpContext.User);
            if (!user.Admin)
            {
                return new UnauthorizedResult();
            }

            return Ok();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]string value)
        {
            var user = Users.GetUserFromPrincipal(HttpContext.User);
            if (!user.Admin)
            {
                return new UnauthorizedResult();
            }

            var u = Users.GetById(id);

            if (u != null)
            {
                return Ok(u);
            }
            else
            {
                return new NotFoundResult();
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = Users.GetUserFromPrincipal(HttpContext.User);
            if (!user.Admin)
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
