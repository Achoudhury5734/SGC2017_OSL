using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OSL.AdminPortal.Models;
using OSL.AdminPortal.Infrastructure;

namespace OSL.AdminPortal.Controllers
{
    //[AuthorizeRoles("SuperUsers")]
    public class SecurityController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public SecurityController()
        {
        }

        public SecurityController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public ActionResult ListUsers(string id)
        {
            IQueryable<vwUserList> userList = ManageUsers.UserList();
            return View(userList);
        }

        public ActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser existingUser = UserManager.FindByEmail(model.Email);
                if (existingUser != null)
                {
                    TempData["NewUser"] = model.Email;
                    return RedirectToAction("UserAlreadyExists", "Security");
                }

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

                // Does email already exist?

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    ManageRoles.AddUserToRole(model.Email, App_Role.Users);
                    if (model.MakeSuperUser)
                    {
                        ManageRoles.AddUserToRole(model.Email, App_Role.SuperUsers);
                    }
                    // Don't sign them in.
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    TempData["NewUser"] = model.Email;
                    return RedirectToAction("UserCreated", "Security");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult UserCreated()
        {
            ViewBag.FullName = TempData["NewUser"].ToString();
            return View();
        }

        public ActionResult UserAlreadyExists()
        {
            ViewBag.FullName = TempData["NewUser"].ToString();
            return View();
        }

        public ActionResult DeleteUser(string id)
        {
            RegisterViewModel rvm = new RegisterViewModel();
            var user = UserManager.FindById(id);

            rvm.Email = user.Email;

            return View(rvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUser(RegisterViewModel model)
        {

            if (ManageRoles.IsUserInRole(model.Email, App_Role.Users))
            {
                ManageRoles.RemoveUserFromRole(model.Email, App_Role.Users);
            }
            if (ManageRoles.IsUserInRole(model.Email, App_Role.SuperUsers))
            {
                ManageRoles.RemoveUserFromRole(model.Email, App_Role.SuperUsers);
            }

            ApplicationUser user = UserManager.FindByEmail(model.Email);
            IdentityResult ir = UserManager.Delete(user);
            TempData["DeletedUser"] = model.Email;
            if (ir.Succeeded)
            {
                TempData["DeletedText"] = " has been deleted.";
            }
            else
            {
                TempData["DeletedText"] = " could not be deleted.";
            }
            return RedirectToAction("UserDeleted", "Security");
        }

        public ActionResult UserDeleted()
        {
            ViewBag.Message = TempData["DeletedUser"].ToString() + TempData["DeletedText"].ToString();
            return View();
        }
    }
}