﻿using System;
using System.Data.SqlClient;

namespace OSL.MobileAppService.Models
{
    public class Donation
    {
        public int Id { get; set; }
        // User Model
        public string Donor { get; set; }
        // User Model
        public string Recipient { get; set; }
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
            // User Model
            Donor = reader["Donor"].ToString();
            // User Model
            Recipient = reader["Recipient"].ToString();
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
