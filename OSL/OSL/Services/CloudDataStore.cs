using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Plugin.Connectivity;
using System.Net.Http.Headers;

namespace OSL
{
    public class CloudDataStore : IDataStore<PickupItem>
    {
        HttpClient client;
        IEnumerable<PickupItem> items;

        public CloudDataStore()
        {
        }

        public async Task<IEnumerable<PickupItem>> GetPickupItemsAsync(bool forceRefresh = false)
        {
            if (forceRefresh && CrossConnectivity.Current.IsConnected)
            {
                //Below line required for debugging only
                //App.ApiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ilg1ZVhrNHh5b2pORnVtMWtsMll0djhkbE5QNC1jNTdkTzZRR1RWQndhTmsifQ.eyJpc3MiOiJodHRwczovL2xvZ2luLm1pY3Jvc29mdG9ubGluZS5jb20vNjcxYjE0NGYtZjc1OS00ODYyLWIzMTktZmU4YjYxNjAyMTUyL3YyLjAvIiwiZXhwIjoxNTA3NTgzMDAzLCJuYmYiOjE1MDc0OTY2MDMsImF1ZCI6ImMyZmRjN2U1LWE2NDUtNDc4Ni04ZDY1LTZhMWQ3OGUxZDRlZCIsIm9pZCI6Ijk4NjczYjQ5LTI3ZTgtNGM3OC04N2RmLTA1NzZkZThmOTMyOCIsInN1YiI6Ijk4NjczYjQ5LTI3ZTgtNGM3OC04N2RmLTA1NzZkZThmOTMyOCIsImVtYWlscyI6WyJkYXZlc3RlcnM4MUBnbWFpbC5jb20iXSwidGZwIjoiQjJDXzFfU2lnblVwU2lnbkluIiwic2NwIjoidXNlciIsImF6cCI6IjE3NzE4NjllLWYzYWEtNGQxNi1iZDFhLWFmMmYwZTY2MmJjZCIsInZlciI6IjEuMCJ9.DxyAPAEVXTp5i8bCHmWo9_7Bqr2KryYgrlCmv6mk7GDVtZL1H6vAZn_aMFXx4UbjbWnBvl45KBqJFma2TtHfEzrA7tMrdOvkYkExo3SPA5p8TwH4w5qiRYh5HMJlR9kTQBr1KZKwumfPGL_sejm2hOWKtAt7piJGZq9dOPwVRSbpTyVeYkLAzaMhulkqwW3hW4RcsbKabdV0IPVVajmhbpxmaz4BrpxM05Fc_AJVQOpVX6Wdd_NUhoDKiRq-x3Xst_FoLzjYnhSr5BKnDzpkICUGLeFzTuTEbxfVkhpgL-_NbX4Rzq8BkNvSnuW8ZilBDslJeF0bG_cgxaaDEmzckA");
                var json = await App.ApiClient.GetStringAsync($"api/donations/status/listed");
                
                items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<PickupItem>>(json));
            }

            return items;
        }

        public async Task<IEnumerable<PickupItem>> GetFilteredItemsAsync(int range, double? Lat, double? Long, bool forceRefresh = true)
        {
            if (forceRefresh && CrossConnectivity.Current.IsConnected)
            {
                var data = new { Miles = range, Latitude = Lat, Longitude = Long };
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                var response = await App.ApiClient.PostAsync("api/donations/nearby/", content);
                var results = response.Content.ReadAsStringAsync().Result;

                items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<PickupItem>>(results));
            }

            return items;
        }

        public async Task<PickupItem> GetPickupItemAsync(string id)
        {
            if (id != null && CrossConnectivity.Current.IsConnected)
            {
                var json = await client.GetStringAsync($"api/pickupitem/{id}");
                return await Task.Run(() => JsonConvert.DeserializeObject<PickupItem>(json));
            }

            return null;
        }

        public async Task<bool> AddPickupItemAsync(PickupItem item)
        {
            if (item == null || !CrossConnectivity.Current.IsConnected)
                return false;

            var serializedItem = JsonConvert.SerializeObject(item);

            var response = await client.PostAsync($"api/pickupitem", new StringContent(serializedItem, Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AcceptPickupItemAsync(PickupItem item)
        {
            var response = await App.ApiClient.PutAsync($"api/donations/{item.Id}/accept", null);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdatePickupItemAsync(PickupItem item)
        {
            if (item == null || item.Id == 0L || !CrossConnectivity.Current.IsConnected)
                return false;

            var serializedItem = JsonConvert.SerializeObject(item);
            var buffer = Encoding.UTF8.GetBytes(serializedItem);
            var byteContent = new ByteArrayContent(buffer);

            var response = await client.PutAsync(new Uri($"api/pickupitem/{item.Id}"), byteContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeletePickupItemAsync(string id)
        {
            if (string.IsNullOrEmpty(id) && !CrossConnectivity.Current.IsConnected)
                return false;

            var response = await client.DeleteAsync($"api/pickupitem/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}
