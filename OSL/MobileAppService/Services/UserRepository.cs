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
        private SqlConnectionStringBuilder builder;

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

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand($"SELECT * FROM User WHERE Oid == {oid}", connection))
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

        public User Create(User user)
        {
            var query = $"SELECT * FROM User WHERE Oid == {oid}";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand($"SELECT * FROM User WHERE Oid == {oid}", connection))
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

        public IEnumerable<User> Get()
        {
            var query = "SELECT * FROM User";
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
            var query = $"SELECT * FROM User WHERE Id = '{id}'";

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

        public void UpdateUser(User user)
        {
            var query = $"UPDATE User SET Person_Name = '{user.Person_Name}', Verified = {user.Verified}, Admin = {user.Admin}, Status = '{user.Status.ToString()}', Phone_GUID = '{user.Phone_GUID}', Phone_Number = '{user.Phone_Number}', Organization_Name = '{user.Organization_Name}', Organization_Address_Line1 = '{user.Organization_Address_Line1}', Organization_Address_Line2 = '{user.Organization_Address_Line2}', Organization_City = '{user.Organization_City}', Organization_State = '{user.Organization_State}', Organization_PostalCode = '{user.Organization_PostalCode}', Organization_Country = '{user.Organization_Country}' WHERE Id = '{user.Id}'";

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
        }

        public void DeleteById(int id)
        {
            var query = $"UPDATE User SET 'Status' = '{UserStatus.Inactive.ToString()}' WHERE Id = '{id}'";

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
