using System;
using System.Data.SqlClient;
using OSL.MobileAppService.Services;

namespace OSL.MobileAppService.Models
{
    public class Donation
    {
        public int Id { get; set; }
        public int DonorId { get; set; }
        public User Donor { get; set; }
        public int RecipientId { get; set; }
        public User Recipient { get; set; }
        public string Title { get; set; }
        public DonationType Type { get; set; }
        public DonationStatus Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime StatusUpdated { get; set; }
        public DateTime Expiration { get; set; }
        public int Amount { get; set; }
        public string Picture { get; set; }


        public Donation(SqlDataReader reader)
        {
            Id = int.Parse(reader["Id"].ToString());
            // User Model Id
            if (reader["Donor"].ToString() != "") {
                DonorId = int.Parse((reader["Donor"].ToString()));
            }
            // User Model Id
            if (reader["Recipient"].ToString() != "") {
                RecipientId = int.Parse((reader["Recipient"].ToString()));
            }
            Title = reader["Title"].ToString();
            if (reader["Type"].ToString() != "") {
                Type = Enum.Parse<DonationType>(reader["Type"].ToString());
            }
            Status = Enum.Parse<DonationStatus>(reader["Status"].ToString());
            Created = DateTime.Parse(reader["Created"].ToString());
            Updated = DateTime.Parse(reader["Updated"].ToString());
            StatusUpdated = DateTime.Parse(reader["StatusUpdated"].ToString());
            if (reader["Expiration"].ToString() != "") {
                Expiration = DateTime.Parse(reader["Expiration"].ToString());
            }
            Amount = int.Parse(reader["Amount"].ToString());
            Picture = reader["Picture"].ToString();
        }
    }
}
