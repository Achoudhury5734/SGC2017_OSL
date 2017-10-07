using OSL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OSL.Services
{
    public class DonationRepository
    {
        public Task<Donation> GetDonationAsync(string donationId)
        {
            return Task.FromResult(new Donation());
        }

        public Task SaveDonationAsync(Donation donation)
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
