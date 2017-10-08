using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OSL.MobileAppService.Models;

namespace OSL.MobileAppService.Controllers
{
    [Route("[controller]")]
    public class UserWebController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("details/{id}")]
        public IActionResult Details(string id)
        {
            var user = new User();
            user.Email = "asdf";
            return View(user);
        }

    }
}