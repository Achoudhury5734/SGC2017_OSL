using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

using OSL.MobileAppService.Models;

namespace OSL.MobileAppService.Services
{
    public class UserRepository
    {
        private readonly SqlConnectionStringBuilder builder;

        public UserRepository(IConfigurationRoot configuration)
        {
            try
            {
                builder = new SqlConnectionStringBuilder()
                {
                    DataSource = configuration["Database:DataSource"],
                    UserID = configuration["Database:UserID"],
                    Password = configuration["Database:Password"],
                    InitialCatalog = configuration["Database:InitialCatalog"]
                };
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public User GetUserFromPrincipal(ClaimsPrincipal principal)
        {
            var oid = principal.Claims.First(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            if (oid == null) {
                return null;
            }

            var query = $"SELECT * FROM [User] WHERE [Oid] = '{oid}' AND [Status] = 'Active'";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            return new User(reader);
                        }
                    }
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            return null;
        }

        public bool IsActiveAdmin(User user)
        {
            if (user != null && user.Admin && user.Status == UserStatus.Active) {
                return true;
            } else {
                return false;
            }
        }

        public bool IsActiveUser(User user)
        {
            if (user != null && user.Status == UserStatus.Active) {
                return true;
            } else {
                return false;
            }
        }

        public bool Create(User user)
        {
            var query = $"INSERT INTO [User] (Oid, Email, Person_Name, Verified, Admin, Status, Phone_Number, Organization_Name, " +
                "Organization_Address_Line1, Organization_Address_Line2, Organization_City, Organization_State, Organization_PostalCode, Organization_Country) VALUES " +
                    $"('{user.Oid}', " +
                    $"'{user.Email}', " +
                    $"'{user.Person_Name}', " +
                    "0, " +
                    "0, " +
                    "'Inactive', " +
                    $"'{user.Phone_Number}', " +
                    $"'{user.Organization_Name}', " +
                    $"'{user.Organization_Address_Line1}', " +
                    $"'{user.Organization_Address_Line2}', " +
                    $"'{user.Organization_City}', " +
                    $"'{user.Organization_State}', " +
                    $"'{user.Organization_PostalCode}', " +
                    $"'{user.Organization_Country}')";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    return command.ExecuteNonQuery() == 1;
                }
            }
        }

        public IEnumerable<User> Get()
        {
            var query = "SELECT * FROM [User]";
            var users = new List<User>();

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
            
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var user = new User(reader);

                        users.Add(user);
                    }
                }
            }

            return users;
        }

        public User GetById(int id)
        {
            var query = $"SELECT * FROM [User] WHERE [Id] = '{id}'";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        return new User(reader);
                    }
                }
            }

            return null;
        }

        public bool UpdateUser(int id, User user)
        {
            var admin = user.Admin ? 1 : 0;
            var verified = user.Verified ? 1 : 0;

            var query = $"UPDATE [User] SET " +
                    $"[Person_Name] = '{user.Person_Name}', " +
                    $"[Verified] = {verified}, " +
                    $"[Admin] = {admin}, " +
                    $"[Status] = '{user.Status.ToString("F")}', " +
                    $"[Phone_GUID] = '{user.Phone_GUID}', " +
                    $"[Phone_Number] = '{user.Phone_Number}', " +
                    $"[Organization_Name] = '{user.Organization_Name}', " +
                    $"[Organization_Address_Line1] = '{user.Organization_Address_Line1}', " +
                    $"[Organization_Address_Line2] = '{user.Organization_Address_Line2}', " +
                    $"[Organization_City] = '{user.Organization_City}', " +
                    $"[Organization_State] = '{user.Organization_State}', " +
                    $"[Organization_PostalCode] = '{user.Organization_PostalCode}', " +
                    $"[Organization_Country] = '{user.Organization_Country}' " +
                $"WHERE [Id] = {id}";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    var res = command.ExecuteNonQuery();

                    return res == 1;
                }
            }
        }

        public void DeleteById(int id)
        {
            var query = $"UPDATE [User] SET [Status] = '{UserStatus.Inactive.ToString("F")}' WHERE [Id] = {id}";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
