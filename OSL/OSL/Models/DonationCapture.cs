using System;

namespace OSL.Models
{
    public class DonationCapture
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public int Amount { get; set; }
        public DateTime Expiration { get; internal set; }
        public byte[] Image { get; set; }

    }
}
