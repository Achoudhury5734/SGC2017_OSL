using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OSL.AdminPortal.Infrastructure
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        private readonly string[] allowedRoles;

        public AuthorizeRolesAttribute(params string[] roles)
        {
            this.allowedRoles = roles;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string userName = httpContext.User.Identity.Name;
            bool authorize = false;
            foreach (var role in allowedRoles)
            {
                if (ManageRoles.IsUserInRole(userName, role))
                {
                    authorize = true;
                    break;
                }
            }
            return authorize;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }
    }
}