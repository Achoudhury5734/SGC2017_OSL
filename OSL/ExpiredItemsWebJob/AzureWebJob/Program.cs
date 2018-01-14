using System;
using System.Data.SqlClient;

namespace AzureWebJob
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnectionStringBuilder builder;

            try
            {
                builder = new SqlConnectionStringBuilder();

                builder.DataSource = "secondhelping.database.windows.net";
                builder.UserID = Environment.GetEnvironmentVariable("Database:UserID");
                builder.Password = Environment.GetEnvironmentVariable("Database:Password");
                builder.InitialCatalog = "secondhelpingDB";

                var query = "UPDATE [Donation] SET [Status] = 3, " +
                            "[Updated] = @Updated, [StatusUpdated] = @StatusUpdated " +
                            "WHERE [Status] = 0 AND [Expiration] < @Now";
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Updated", DateTime.Now);
                        command.Parameters.AddWithValue("@StatusUpdated", DateTime.Now);
                        command.Parameters.AddWithValue("@Now", DateTime.Now);
                        command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                }
            }
            catch (SqlException error)
            {
                Console.WriteLine(error);
            }
        }
    }
}
