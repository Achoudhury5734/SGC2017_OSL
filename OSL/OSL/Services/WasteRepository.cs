using System;
using OSL.Models;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;

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

        public async Task<int[]> GetDonorStats()
        {
            var response = await App.ApiClient.GetAsync("api/donations/donor/me/stats");

            if (response.StatusCode != HttpStatusCode.OK)
                return null;
            
            var result = response.Content.ReadAsStringAsync().Result;
            var stats = JsonConvert.DeserializeObject<int[]>(result);

            return stats;
        }
    }
}
