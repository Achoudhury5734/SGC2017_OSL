using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Plugin.Connectivity;

namespace OSL
{
    public class CloudDataStore : IDataStore<PickupItem>
    {
        HttpClient client;
        IEnumerable<PickupItem> items;

        public CloudDataStore()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri($"{App.BackendUrl}/");

            items = new List<PickupItem>();
        }

        public async Task<IEnumerable<PickupItem>> GetPickupItemsAsync(bool forceRefresh = false)
        {
            if (forceRefresh && CrossConnectivity.Current.IsConnected)
            {
                var json = await client.GetStringAsync($"api/pickupitem");
                items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<PickupItem>>(json));
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
            var response = await App.ApiClient.PostAsync($"api/donations/{item.Id}/accept", null);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdatePickupItemAsync(PickupItem item)
        {
            if (item == null || item.Id == null || !CrossConnectivity.Current.IsConnected)
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
