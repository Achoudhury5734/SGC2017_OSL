using OSL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSL.Services
{
    public class DonationRepository
    {
        public DonationRepository()
        {
            Donations = new List<Donation>();
        }

        public List<Donation> Donations { get; set; }

        public Task<Donation> GetDonationAsync(int donationId)
        {
            return Task.FromResult(Donations.Find(d => d.Id == donationId));
        }

        public Task<IEnumerable<Donation>> GetDonationsByAccountAsync(string accountId)
        {
            return Task.FromResult(Donations.AsEnumerable());
        }

        public Task SaveDonationAsync(DonationCapture donation)
        {
            return Task.CompletedTask;
        }

        public Task CompleteDonationAsync(string donationId)
        {
            return Task.CompletedTask;
        }

        public Task WasteDonationAsync(string donationId)
        {
            return Task.CompletedTask;
        }

        public Task AcceptDonation(string accountId, string donationId)
        {
            return Task.CompletedTask;
        }
    }
}
