using System;

namespace OSL.Models
{
    public class Donation
    {
        public int Id { get; set; }
        public User Donor { get; set; }
        public User Recipient { get; set; }
        public string Title { get; set; }
        public DonationType Type { get; set; }
        public DonationStatus Status { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? StatusUpdated { get; set; }
        public DateTime? Expiration { get; set; }
        public int Amount { get; set; }
        public string PictureUrl { get; set; }
    }
}