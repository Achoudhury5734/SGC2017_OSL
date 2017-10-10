using System;
using OSL.Models;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using System.Linq;

namespace OSL.Services
{
    public class WasteRepository
    {
        public async Task<Donation> CreateWaste(Donation donation)
        {
            var content = new StringContent(JsonConvert.SerializeObject(donation), Encoding.UTF8, "application/json");
            var response = await App.ApiClient.PostAsync($"api/donations", content);

            if (response.StatusCode == HttpStatusCode.OK)
                return donation;
            
            return null;
        }

        private async Task<IEnumerable<Donation>> GetDonorItems(int donorId)
        {
            var response = await App.ApiClient.GetAsync($"api/donations");
            if (response.StatusCode != HttpStatusCode.OK)
                return null;
            var result = response.Content.ReadAsStringAsync().Result;
            var items = JsonConvert.DeserializeObject<IEnumerable<Donation>>(result);
            return from item in items where item.DonorId == donorId select item;
        }

        public async Task<int> GetDonorWaste(int donorId)
        {
            var items = await GetDonorItems(donorId);
            var queryWaste = from item in items
                             where item.Status == DonationStatus.Wasted
                             select item.Amount;
            return queryWaste.Sum();
        }

        public async Task<int> GetDonorComplete(int donorId)
        {
            var items = await GetDonorItems(donorId);
            var queryComplete = from item in items
                                where item.Status == DonationStatus.Completed
                                select item.Amount;
            return queryComplete.Sum();
        }
    }
}
