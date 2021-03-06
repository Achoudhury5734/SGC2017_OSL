﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using OSL.Models;
using Plugin.Connectivity;

namespace OSL
{
    public class CloudDataStore : IDataStore<Donation>
    {
        IEnumerable<Donation> items;

        public async Task<IEnumerable<Donation>> GetPickupItemsAsync(bool forceRefresh = false)
        {
            if (forceRefresh && CrossConnectivity.Current.IsConnected)
            {
                var json = await App.ApiClient.GetStringAsync($"api/donations/status/listed"); 
                items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<Donation>>(json));
            }

            return items;
        }

        public async Task<IEnumerable<Donation>> GetFilteredItemsAsync(int range, double? Lat, double? Long, bool forceRefresh = true)
        {
            if (forceRefresh && CrossConnectivity.Current.IsConnected)
            {
                var data = new { Miles = range, Latitude = Lat, Longitude = Long };
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                var response = await App.ApiClient.PostAsync("api/donations/nearby/", content);
                var results = response.Content.ReadAsStringAsync().Result;

                items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<Donation>>(results));
            }

            return items;
        }

        public async Task<Donation> GetPickupItemAsync(string id)
        {
            await Task.FromResult<Donation>(null);
            throw new NotSupportedException();
        }

        public async Task<bool> AddPickupItemAsync(Donation item)
        {
            await Task.FromResult(false);
            throw new NotSupportedException();
        }

        public async Task<bool> AcceptPickupItemAsync(Donation item)
        {
            var response = await App.ApiClient.PutAsync($"api/donations/{item.Id}/accept", null);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdatePickupItemAsync(Donation item)
        {
            await Task.FromResult(0);
            throw new NotSupportedException();
        }

        public async Task<bool> DeletePickupItemAsync(string id)
        {
            await Task.FromResult(0);
            throw new NotSupportedException();
        }
    }
}
