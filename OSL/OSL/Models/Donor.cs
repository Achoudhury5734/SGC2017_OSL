
using Newtonsoft.Json;

namespace OSL
{
    //I think this class can be deprecated. Please use User instead.
    public class Donor
    {
        [JsonProperty("lat")]
        public long Lat { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("admin")]
        public bool Admin { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("oid")]
        public string Oid { get; set; }

        [JsonProperty("long")]
        public double Long { get; set; }

        [JsonProperty("organization_Address_Line1")]
        public string OrganizationAddressLine1 { get; set; }

        [JsonProperty("organization_Address_Line2")]
        public string OrganizationAddressLine2 { get; set; }

        [JsonProperty("organization_PostalCode")]
        public string OrganizationPostalCode { get; set; }

        [JsonProperty("organization_Country")]
        public string OrganizationCountry { get; set; }

        [JsonProperty("organization_City")]
        public string OrganizationCity { get; set; }

        [JsonProperty("organization_Name")]
        public string OrganizationName { get; set; }

        [JsonProperty("person_Name")]
        public string PersonName { get; set; }

        [JsonProperty("phone_Number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("organization_State")]
        public string OrganizationState { get; set; }

        [JsonProperty("phone_GUID")]
        public string PhoneGUID { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("verified")]
        public bool Verified { get; set; }
    }
}
