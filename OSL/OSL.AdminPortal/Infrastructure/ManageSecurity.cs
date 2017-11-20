using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OSL.AdminPortal.Models;
using System.Data.SqlClient;
using System.Data;

namespace OSL.AdminPortal.Infrastructure
{
    public class App_Role
    {
        public App_Role(string value)
        {
            Value = value;
        }

        public static string Value { get; set; }

        public static string SuperUsers
        {
            get
            {
                return "SuperUsers";
            }
        }
        public static string Users
        {
            get
            {
                return "Users";
            }
        }
    }

    public class ManageUsers
    {
        static OSLdb context = new OSLdb();

        public static IQueryable<vwUserList> UserList()
        {
            return context.vwUserLists.OrderBy(u => u.UserName);
        }
    }

    public class ManageRoles
    {
        static OSLdb context = new OSLdb();

        public static void AddUserToRole(string user, string role)
        {
            // User already in role, so we are good
            if (IsUserInRole(user, role))
            {
                return;
            }

            var theUser = context.AspNetUsers.Where(u => u.UserName == user).SingleOrDefault();
            if (theUser == null)
            {
                throw new Exception($"AddUserToRole issue occurred. The specified user {user} does not exist.");
            }
            var theRole = context.AspNetRoles.Where(r => r.Name == role).SingleOrDefault();
            if (theRole == null)
            {
                throw new Exception($"AddUserToRole issue occurred. The specified role {role} does not exist.");
            }

            try
            {
                theUser.AspNetRoles.Add(theRole);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("AddUserToRole exception occurred.", ex);
            }
        }

        public static bool IsUserInRole(string userName, string role)
        {
            var query = context.vwUserRoles.Where(ur => ur.UserName == userName && ur.RoleName == role).Count();
            return (query > 0);
        }

        public static void RemoveUserFromRole(string user, string role)
        {
            // User already not in role, so we are good
            if (!IsUserInRole(user, role))
            {
                return;
            }

            var theUser = context.AspNetUsers.Where(u => u.UserName == user).SingleOrDefault();
            if (theUser == null)
            {
                throw new Exception($"RemoveUserFromRole issue occurred. The specified user {user} does not exist.");
            }
            var theRole = context.AspNetRoles.Where(r => r.Name == role).SingleOrDefault();
            if (theRole == null)
            {
                throw new Exception($"RemoveUserFromRole issue occurred. The specified role {role} does not exist.");
            }

            try
            {
                theUser.AspNetRoles.Remove(theRole);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("RemoveUserFromRole exception occurred.", ex);
            }
        }
     }
}
 