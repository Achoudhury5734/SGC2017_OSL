using Newtonsoft.Json;
using System;

namespace OSL
{
    public class PickupItem
    {
        [JsonProperty("donorId")]
        public long DonorId { get; set; }

        [JsonProperty("recipient")]
        public object Recipient { get; set; }

        [JsonProperty("created")]
        public string Created { get; set; }

        [JsonProperty("amount")]
        public long Amount { get; set; }

        [JsonProperty("donor")]
        public Donor Donor { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("expiration")]
        public DateTime? Expiration { get; set; }

        [JsonProperty("PictureUrl")]
        public string PictureUrl { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("recipientId")]
        public object RecipientId { get; set; }

        [JsonProperty("statusUpdated")]
        public string StatusUpdated { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("updated")]
        public string Updated { get; set; }
    }
}
