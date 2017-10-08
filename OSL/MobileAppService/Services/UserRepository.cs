using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Geocoding.Google;
using Microsoft.Extensions.Configuration;

using OSL.MobileAppService.Models;

namespace OSL.MobileAppService.Services
{
    public class UserRepository
    {
        private readonly SqlConnectionStringBuilder builder;
        private readonly IConfigurationRoot configuration;

        public UserRepository(IConfigurationRoot configuration)
        {
            this.configuration = configuration;

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

        public async Task<bool> Create(User user)
        {
            var geocoder = new GoogleGeocoder();
            var addresses = await geocoder.GeocodeAsync($"{user.Organization_Address_Line1} {user.Organization_Address_Line2}, {user.Organization_City}, {user.Organization_State}, {user.Organization_PostalCode}");

            var query = $"INSERT INTO [User] (Oid, Email, Person_Name, Verified, Admin, Status, Phone_Number, Organization_Name, " +
                "Organization_Address_Line1, Organization_Address_Line2, Organization_City, Organization_State, Organization_PostalCode, Organization_Country, Lat, Long) VALUES " +
                    $"('{user.Oid}', " +
                    $"'{user.Email}', " +
                    $"'{user.Person_Name}', " +
                    "0, " +
                    "0, " +
                    $"{UserStatus.Active.ToString()}, " +
                    $"'{user.Phone_Number}', " +
                    $"'{user.Organization_Name}', " +
                    $"'{user.Organization_Address_Line1}', " +
                    $"'{user.Organization_Address_Line2}', " +
                    $"'{user.Organization_City}', " +
                    $"'{user.Organization_State}', " +
                    $"'{user.Organization_PostalCode}', " +
                    $"'{user.Organization_Country}', " +
                    $"{addresses.First().Coordinates.Latitude}, " +
                    $"{addresses.First().Coordinates.Longitude})";

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
            var query = $"SELECT * FROM [User] WHERE [Id] = {id}";

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

        public async Task<bool> UpdateUser(int id, User user)
        {
            var admin = user.Admin ? 1 : 0;
            var verified = user.Verified ? 1 : 0;

            var geocoder = new GoogleGeocoder();
            var addresses = await geocoder.GeocodeAsync($"{user.Organization_Address_Line1} {user.Organization_Address_Line2}, {user.Organization_City}, {user.Organization_State}, {user.Organization_PostalCode}");

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
                    $"[Organization_Country] = '{user.Organization_Country}', " +
                    $"[Lat] = {addresses.First().Coordinates.Latitude}, " +
                    $"[Long] = {addresses.First().Coordinates.Longitude} " +
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

        public void ActivateById(int id)
        {
            var query = $"UPDATE [User] SET [Status] = '{UserStatus.Active.ToString("F")}' WHERE [Id] = {id}";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeactivateById(int id)
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
