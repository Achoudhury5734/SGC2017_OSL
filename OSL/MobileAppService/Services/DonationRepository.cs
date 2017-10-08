using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
                connection.Open();
            
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var donation = new Donation(reader);
                        donations.Add(donation);
                    }
                }
            }
            return donations;
        }

        public Donation Get(int Id)
        {
            var query = "SELECT * Donations WHERE Id = @Id";
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", Id);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        return new Donation(reader);
                    }
                }
            }
            return null;
        }
    }
}
