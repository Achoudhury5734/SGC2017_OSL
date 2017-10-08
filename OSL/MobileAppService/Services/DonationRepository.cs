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
            catch (SqlException error)
            {
                Console.WriteLine(error);
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

        public Donation GetById(int Id)
        {
            var query = "SELECT * FROM [Donation] WHERE [Id] = @Id";
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", Id);
                    SqlDataReader reader = command.ExecuteReader();
                    command.Parameters.Clear();
                    while (reader.Read())
                    {
                        return new Donation(reader);
                    }
                }
            }
            return null;
        }

        public Donation Create(Donation donation)
        {
            var query = "INSERT INTO [Donation] (DonorId, Title, Type, Status, " +
                        "Created, Updated, StatusUpdated, Expiration, Amount, " +
                        "PictureUrl) OUTPUT INSERTED.Id VALUES ( " +
                            "@DonorId, " +
                            "@Title, " +
                            "@Type, " +
                            "@Status, " +
                            "@Created, " +
                            "@Updated, " +
                            "@StatusUpdated, " +
                            "@Expiration, " +
                            "@Amount, " +
                            "@PictureUrl" + 
                        ")";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DonorId", donation.DonorId);
                    command.Parameters.AddWithValue("@Title", donation.Title);
                    command.Parameters.AddWithValue("@Type", donation.Type);
                    command.Parameters.AddWithValue("@Status", donation.Status);
                    command.Parameters.AddWithValue("@Created", donation.Created);
                    command.Parameters.AddWithValue("@Updated", donation.Updated);
                    command.Parameters.AddWithValue("@StatusUpdated", donation.StatusUpdated);
                    command.Parameters.AddWithValue("@Expiration", donation.Expiration);
                    command.Parameters.AddWithValue("@Amount", donation.Amount);
                    command.Parameters.AddWithValue("@PictureUrl", donation.PictureUrl);

                    try
                    {
                        var Id = int.Parse(command.ExecuteScalar().ToString());
                        command.Parameters.Clear();
                        if (Id > 0) {
                            return GetById(Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }


                }
            }
            return null;
        }
    }
}
