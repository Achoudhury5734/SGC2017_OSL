
using System;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OSL.MobileAppService.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Oid { get; set; }
        public string Email { get; set; }
        public string Person_Name { get; set; }
        public bool Verified { get; set; }
        public bool Admin { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public UserStatus Status { get; set; }

        public string Phone_GUID { get; set; }
        public string Phone_Number { get; set; }
        public string Organization_Name { get; set; }
        public string Organization_Address_Line1 { get; set; }
        public string Organization_Address_Line2 { get; set; }
        public string Organization_City { get; set; }
        public string Organization_State { get; set; }
        public string Organization_PostalCode { get; set; }
        public string Organization_Country { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }

        public User() {}

        public User(SqlDataReader reader)
        {
            Id = int.Parse(reader["Id"].ToString());
            Oid = reader["Oid"].ToString();
            Email = reader["Email"].ToString();
            Person_Name = reader["Person_Name"].ToString();
            Verified = reader["Verified"].ToString() == "True";
            Admin = reader["Admin"].ToString() == "True";
            Status = Enum.Parse<UserStatus>(reader["Status"].ToString());
            Phone_GUID = reader["Phone_GUID"].ToString();
            Phone_Number = reader["Phone_Number"].ToString();
            Organization_Name = reader["Organization_Name"].ToString();
            Organization_Address_Line1 = reader["Organization_Address_Line1"].ToString();
            Organization_Address_Line2 = reader["Organization_Address_Line2"].ToString();
            Organization_City = reader["Organization_City"].ToString();
            Organization_State = reader["Organization_State"].ToString();
            Organization_PostalCode = reader["Organization_PostalCode"].ToString();
            Organization_Country = reader["Organization_Country"].ToString();

            if (reader["Lat"].ToString() != "") {
                Lat = double.Parse(reader["Lat"].ToString());
            }
            if (reader["Long"].ToString() != "")
            {
                Long = double.Parse(reader["Long"].ToString());
            }
        }
    }
}
