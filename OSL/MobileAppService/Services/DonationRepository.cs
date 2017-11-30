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

        public IEnumerable<Donation> GetAll()
        {
            var query = "SELECT * FROM [Donation]";
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

        public IEnumerable<Donation> GetListed()
        {
            var query = $"SELECT * FROM [Donation] WHERE [Status] = {(int)DonationStatus.Listed} " +
                        "ORDER BY [Created] DESC";
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

        public IEnumerable<Donation> GetByDonorId(int DonorId)
        {
            var query = "SELECT * FROM [Donation] WHERE [DonorId] = @DonorId " +
                        "AND [Title] != @UserEnteredWaste ORDER BY [Created] DESC";
            var donations = new List<Donation>();

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DonorId", DonorId);
                    command.Parameters.AddWithValue("@UserEnteredWaste", "UserEnteredWaste");
                    SqlDataReader reader = command.ExecuteReader();
                    command.Parameters.Clear();
                    while (reader.Read())
                    {
                        var donation = new Donation(reader);
                        donations.Add(donation);
                    }
                }
            }
            return donations;
        }

        public IEnumerable<Donation> GetByDonorIdWithStatus(int DonorId, int Status)
        {
            var query = "SELECT * FROM [Donation] WHERE [DonorId] = @DonorId " +
                        "AND [Status] = @Status AND [Title] != @UserEnteredWaste " +
                        "ORDER BY [Created] DESC";
            var donations = new List<Donation>();
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DonorId", DonorId);
                    command.Parameters.AddWithValue("@Status", Status);
                    command.Parameters.AddWithValue("@UserEnteredWaste", "UserEnteredWaste");
                    SqlDataReader reader = command.ExecuteReader();
                    command.Parameters.Clear();
                    while (reader.Read())
                    {
                        var donation = new Donation(reader);
                        donations.Add(donation);
                    }
                }
            }
            return donations;   
        }

        public IEnumerable<Donation> GetByRecipientId(int RecipientId)
        {
            var query = "SELECT * FROM [Donation] WHERE [RecipientId] = @RecipientId " +
                        "ORDER BY [Updated] DESC";
            var donations = new List<Donation>();

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RecipientId", RecipientId);
                    SqlDataReader reader = command.ExecuteReader();
                    command.Parameters.Clear();
                    while (reader.Read())
                    {
                        var donation = new Donation(reader);
                        donations.Add(donation);
                    }
                }
            }
            return donations;
        }

        public IEnumerable<Donation> GetByRecipientIdWithStatus(int RecipientId, int Status)
        {
            var query = "SELECT * FROM [Donation] WHERE [RecipientId] = @RecipientId " +
                        "AND [Status] = @Status ORDER BY [Updated] DESC";
            var donations = new List<Donation>();
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RecipientId", RecipientId);
                    command.Parameters.AddWithValue("@Status", Status);
                    SqlDataReader reader = command.ExecuteReader();
                    command.Parameters.Clear();
                    while (reader.Read())
                    {
                        var donation = new Donation(reader);
                        donations.Add(donation);
                    }
                }
            }
            return donations;
        }

        public IEnumerable<Donation> GetListedWithinDistance(double Lat, double Long, double distance)
        {
            var query = "DECLARE @g geography;" +
                        "SET @g = geography::Point(@Lat, @Long, 4326);" +
                        "SELECT Donation.* FROM [Donation] " +
                        "INNER JOIN " +
                            "(SELECT [Id], @g.STDistance(geography::Point([Lat], [Long], 4326)) as Distance " +
                             "FROM [User]) as T " +
                        "ON Donation.DonorId = T.Id AND T.Distance <= @Distance " +
                        $"WHERE Donation.Status = {(int)DonationStatus.Listed} " +
                        "ORDER BY T.Distance;";
            var donations = new List<Donation>();
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Distance", distance);
                    command.Parameters.AddWithValue("@Lat", Lat);
                    command.Parameters.AddWithValue("@Long", Long);
                    SqlDataReader reader = command.ExecuteReader();
                    command.Parameters.Clear();
                    while (reader.Read())
                    {
                        var donation = new Donation(reader);
                        donations.Add(donation);
                    }
                }
            }
            return donations;
        }

        public IEnumerable<int> GetDonorStats(int DonorId)
        {
            var query = "SELECT [Status], Sum(Amount) FROM Donation WHERE DonorId = @DonorId" +
                        " AND YEAR(Created) = @CurrentYear GROUP BY [Status]";
            var stats = new int[(Enum.GetNames(typeof(DonationStatus)).Length)];

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DonorId", DonorId);
                    command.Parameters.AddWithValue("@CurrentYear", DateTime.Now.Year);
                    SqlDataReader reader = command.ExecuteReader();
                    command.Parameters.Clear();
                    while(reader.Read())
                    {
                        var status = (string)reader["Status"];
                        var sum = (int)reader[1];
                        stats[int.Parse(status)] = sum;
                    }
                }
            }
            return stats;
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
                        var donation = new Donation(reader);
                        return donation;
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
                    catch (Exception)
                    {
                        return null;
                    }


                }
            }
            return null;
        }

        public bool Update(Donation donation)
        {
            var query = "UPDATE [Donation] SET " +
                        $"[Title] = @Title, " +
                        $"[Type] = @Type, " +
                        $"[Updated] = @Updated, " +
                        $"[Expiration] = @Expiration, " +
                        $"[Amount] = @Amount " +
                        $"WHERE [Id] = @Id";
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", donation.Id);
                    command.Parameters.AddWithValue("@Title", donation.Title);
                    command.Parameters.AddWithValue("@Type", donation.Type);
                    command.Parameters.AddWithValue("@Updated", DateTime.Now);
                    command.Parameters.AddWithValue("@Expiration", donation.Expiration);
                    command.Parameters.AddWithValue("@Amount", donation.Amount);
                    var res = command.ExecuteNonQuery();
                    command.Parameters.Clear();
                    return res == 1;
                }
            }
        }

        public void AcceptDonation(int donationId, int recipientId)
        {
            var query = "UPDATE Donation " +
                        $"SET RecipientId = @RecipientId, Status = {(int)DonationStatus.PendingPickup}, " +
                        "Updated = @Updated, StatusUpdated = @StatusUpdated " + 
                        "WHERE [Id] = @Id;";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", donationId);
                    command.Parameters.AddWithValue("@RecipientId", recipientId);
                    command.Parameters.AddWithValue("@Updated", DateTime.Now);
                    command.Parameters.AddWithValue("@StatusUpdated", DateTime.Now);
                    command.ExecuteScalar();
                    command.Parameters.Clear();
                }
            }
        }

        public void CompleteDonation(int donationId)
        {
            var query = "UPDATE Donation " +
                        $"SET Status = {(int)DonationStatus.Completed}, " +
                        "Updated = @Updated, StatusUpdated = @StatusUpdated " +
                        "WHERE [Id] = @Id;";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", donationId);
                    command.Parameters.AddWithValue("@Updated", DateTime.Now);
                    command.Parameters.AddWithValue("@StatusUpdated", DateTime.Now);
                    command.ExecuteScalar();
                    command.Parameters.Clear();
                }
            }
        }

        public void WasteDonation(int donationId)
        {
            var query = "UPDATE Donation " +
                        $"SET Status = {(int)DonationStatus.Wasted}, " +
                        "Updated = @Updated, StatusUpdated = @StatusUpdated, " +
                        "RecipientId = @RecipientId " +
                        "WHERE [Id] = @Id;";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", donationId);
                    command.Parameters.AddWithValue("@StatusUpdated", DateTime.Now);
                    command.Parameters.AddWithValue("@Updated", DateTime.Now);
                    command.Parameters.AddWithValue("@RecipientId", DBNull.Value);
                    command.ExecuteScalar();
                    command.Parameters.Clear();
                }
            }
        }


        public void RelistDonation(int donationId)
        {
            var query = "UPDATE Donation " +
                        $"SET RecipientId = @RecipientId, Status = {(int)DonationStatus.Listed}, " +
                        "Updated = @Updated, StatusUpdated = @StatusUpdated " +
                        "WHERE [Id] = @Id;";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", donationId);
                    command.Parameters.AddWithValue("@RecipientId", DBNull.Value);
                    command.Parameters.AddWithValue("@Updated", DateTime.Now);
                    command.Parameters.AddWithValue("@StatusUpdated", DateTime.Now);
                    command.ExecuteScalar();
                    command.Parameters.Clear();
                }
            }
        }

        public bool RelistDonation(Donation donation)
        {
            var query = "UPDATE [Donation] " + 
                        "SET [RecipientId] = @RecipientId, " +
                        $"[Status] = {(int)DonationStatus.Listed}, " +
                        "[Title] = @Title, [Type] = @Type, [Updated] = @Updated, " +
                        "[Expiration] = @Expiration, [Amount] = @Amount, " +
                        "[StatusUpdated] = @StatusUpdated, [PictureUrl] = @PictureUrl " +
                        "WHERE [Id] = @Id";
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", donation.Id);
                    command.Parameters.AddWithValue("@RecipientId", DBNull.Value);
                    command.Parameters.AddWithValue("@Title", donation.Title);
                    command.Parameters.AddWithValue("@Type", donation.Type);
                    command.Parameters.AddWithValue("@Updated", DateTime.Now);
                    command.Parameters.AddWithValue("@Expiration", donation.Expiration);
                    command.Parameters.AddWithValue("@Amount", donation.Amount);
                    command.Parameters.AddWithValue("@StatusUpdated", DateTime.Now);
                    command.Parameters.AddWithValue("@PictureUrl", donation.PictureUrl);
                    var res = command.ExecuteNonQuery();
                    command.Parameters.Clear();
                    return res == 1; //should affect exactly one row
                }
            }
        }
    }
}
