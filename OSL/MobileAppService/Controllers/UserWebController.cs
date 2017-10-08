using Microsoft.AspNetCore.Mvc;
using OSL.MobileAppService.Services;

namespace OSL.MobileAppService.Controllers
{
    [Route("users")]
    public class UserWebController : Controller
    {
        private readonly UserRepository userRepository;

        public UserWebController(UserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("Index", userRepository.Get());
        }

        [HttpGet("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var user = userRepository.GetById(id);

            if (user != null)
            {
                return View("Edit", user);
            }

            return new NotFoundResult();
        }
    }
}