﻿
namespace OSL.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Oid { get; set; }
        public string Email { get; set; }
        public string Person_Name { get; set; }
        public bool Verified { get; set; }
        public bool Admin { get; set; }
        public string Status { get; set; }
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
    }
}