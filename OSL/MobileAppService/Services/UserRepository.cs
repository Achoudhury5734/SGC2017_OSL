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

            var query = "SELECT * FROM [User] WHERE [Oid] = @Oid AND [Status] = 'Active'";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Oid", oid);
                        SqlDataReader reader = command.ExecuteReader();
                        command.Parameters.Clear();

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

        public async Task<bool> Create(User user)
        {
            IEnumerable<GoogleAddress> addresses = null;

            try
            {
                var geocoder = new GoogleGeocoder();
                addresses = await geocoder.GeocodeAsync($"{user.Organization_Address_Line1} {user.Organization_Address_Line2}, {user.Organization_City}, {user.Organization_State}, {user.Organization_PostalCode}");
            } catch (Exception ex) {
                Console.WriteLine("Error geocoding: " + ex.StackTrace);
            }

            var query = $"INSERT INTO [User] (Oid, Email, Person_Name, Verified, Admin, Status, Phone_Number, Organization_Name, Organization_Address_Line1, " +
                "Organization_Address_Line2, Organization_City, Organization_State, Organization_PostalCode, Organization_Country, Lat, Long) VALUES " +
                    $"(@Oid, " +
                    $"@Email, " +
                    $"@Person_Name, " +
                    "0, " +
                    "0, " +
                    $"'{UserStatus.Active.ToString()}', " +
                    $"@Phone_Number, " +
                    $"@Organization_Name, " +
                    $"@Organization_Address_Line1, " +
                    $"@Organization_Address_Line2, " +
                    $"@Organization_City, " +
                    $"@Organization_State, " +
                    $"@Organization_PostalCode, " +
                    $"@Organization_Country, " +
                    $"@Lat, " +
                    $"@Long)";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Oid", user.Oid);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Person_Name", user.Person_Name ?? "");
                    command.Parameters.AddWithValue("@Phone_Number", user.Phone_Number ?? "");
                    command.Parameters.AddWithValue("@Organization_Name", user.Organization_Name ?? "");
                    command.Parameters.AddWithValue("@Organization_Address_Line1", user.Organization_Address_Line1 ?? "");
                    command.Parameters.AddWithValue("@Organization_Address_Line2", user.Organization_Address_Line2 ?? "");
                    command.Parameters.AddWithValue("@Organization_City", user.Organization_City ?? "");
                    command.Parameters.AddWithValue("@Organization_State", user.Organization_State ?? "");
                    command.Parameters.AddWithValue("@Organization_PostalCode", user.Organization_PostalCode ?? "");
                    command.Parameters.AddWithValue("@Organization_Country", user.Organization_Country ?? "");
                    command.Parameters.AddWithValue("@Lat", addresses?.FirstOrDefault()?.Coordinates.Latitude ?? 0.0);
                    command.Parameters.AddWithValue("@Long", addresses?.FirstOrDefault()?.Coordinates.Longitude ?? 0.0);

                    var res = command.ExecuteNonQuery();
                    command.Parameters.Clear();

                    return res == 1;
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

        public IEnumerable<User> GetUnverified()
        {
            var query = "SELECT * FROM [User] WHERE [Verified] = 0";
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
            var query = "SELECT * FROM [User] WHERE [Id] = @Id";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    SqlDataReader reader = command.ExecuteReader();
                    command.Parameters.Clear();

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

            IEnumerable<GoogleAddress> addresses = null;

            try
            {
                var geocoder = new GoogleGeocoder();
                addresses = await geocoder.GeocodeAsync($"{user.Organization_Address_Line1} {user.Organization_Address_Line2}, {user.Organization_City}, {user.Organization_State}, {user.Organization_PostalCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error geocoding: " + ex.StackTrace);
            }

            var query = $"UPDATE [User] SET " +
                    $"[Person_Name] = @Person_Name, " +
                    $"[Verified] = @Verified, " +
                    $"[Admin] = @Admin, " +
                    $"[Status] = @Status, " +
                    $"[Phone_Number] = @Phone_Number, " +
                    $"[Organization_Name] = @Organization_Name, " +
                    $"[Organization_Address_Line1] = @Organization_Address_Line1, " +
                    $"[Organization_Address_Line2] = @Organization_Address_Line2, " +
                    $"[Organization_City] = @Organization_City, " +
                    $"[Organization_State] = @Organization_State, " +
                    $"[Organization_PostalCode] = @Organization_PostalCode, " +
                    $"[Organization_Country] = @Organization_Country, " +
                    $"[Lat] = @Lat, " +
                    $"[Long] = @Long " +
                $"WHERE [Id] = @Id";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Person_Name", user.Person_Name ?? "");
                    command.Parameters.AddWithValue("@Verified", verified);
                    command.Parameters.AddWithValue("@Admin", admin);
                    command.Parameters.AddWithValue("@Status", user.Status.ToString("F"));
                    command.Parameters.AddWithValue("@Phone_Number", user.Phone_Number ?? "");
                    command.Parameters.AddWithValue("@Organization_Name", user.Organization_Name ?? "");
                    command.Parameters.AddWithValue("@Organization_Address_Line1", user.Organization_Address_Line1 ?? "");
                    command.Parameters.AddWithValue("@Organization_Address_Line2", user.Organization_Address_Line2 ?? "");
                    command.Parameters.AddWithValue("@Organization_City", user.Organization_City ?? "");
                    command.Parameters.AddWithValue("@Organization_State", user.Organization_State ?? "");
                    command.Parameters.AddWithValue("@Organization_PostalCode", user.Organization_PostalCode ?? "");
                    command.Parameters.AddWithValue("@Organization_Country", user.Organization_Country ?? "");
                    command.Parameters.AddWithValue("@Lat", addresses?.FirstOrDefault()?.Coordinates.Latitude ?? 0.0);
                    command.Parameters.AddWithValue("@Long", addresses?.FirstOrDefault()?.Coordinates.Longitude ?? 0.0);
                    command.Parameters.AddWithValue("@Id", id);

                    var res = command.ExecuteNonQuery();
                    command.Parameters.Clear();

                    return res == 1;
                }
            }
        }

        public void ActivateById(int id)
        {
            var query = $"UPDATE [User] SET [Status] = '{UserStatus.Active.ToString("F")}' WHERE [Id] = @Id";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
            }
        }

        public void DeactivateById(int id)
        {
            var query = $"UPDATE [User] SET [Status] = '{UserStatus.Inactive.ToString("F")}' WHERE [Id] = @Id";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
            }
        }

        public void VerifyById(int id)
        {
            var query = $"UPDATE [User] SET [Verified] = 1 WHERE [Id] = @Id";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
            }
        }
    }
}
