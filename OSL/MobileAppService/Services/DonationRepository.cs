using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Microsoft.Extensions.Configuration;

using OSL.MobileAppService.Models;

namespace OSL.MobileAppService.Services
{
    public class DonationRepository
    {
        private SqlConnectionStringBuilder builder;

        public DonationRepository(IConfigurationRoot configuration)
        {
            try
            {
                builder = new SqlConnectionStringBuilder();
                builder.DataSource = configuration["Database:DataSource"];
                builder.UserID = configuration["Database:UserID"];
                builder.Password = configuration["Database:Password"];
                builder.InitialCatalog = configuration["Database:InitialCatalog"];
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        public IEnumerable<Donation> Get()
        {
            var query = "SELECT * FROM Donation";
            var donations = new List<Donation>();


            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                Console.WriteLine("\nQuery data example:");
                Console.WriteLine("=========================================\n");

                connection.Open();
            
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var donation = new Donation {
                            Id = reader["Id"].ToString(),
                            Title = reader["Title"].ToString()
                        };
                        donations.Add(donation);
                    }
                }
            }
            return donations;
        }
    }
}
