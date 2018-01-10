using Newtonsoft.Json;
using OSL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using System.IO;

namespace OSL.Services
{
    public class DonationRepository
    {
        public async Task<IEnumerable<Donation>> GetDonationsByDonorAsync(DonationStatus status)
        {
            var json = await App.ApiClient.GetStringAsync($"api/donations/donor/me/status/{status}");
            return JsonConvert.DeserializeObject<IEnumerable<Donation>>(json);
        }

        public async Task<IEnumerable<Donation>> GetDonationsByRecipientAsync(DonationStatus status)
        {
            var json = await App.ApiClient.GetStringAsync($"api/donations/recipient/me/status/{status}");
            return JsonConvert.DeserializeObject<IEnumerable<Donation>>(json);
        }

        public async Task<bool> SaveDonationAsync(string donationTitle, MediaFile mediaFile, int quantity, string donationType, DateTime expirationDate, TimeSpan expirationTime)
        {
            byte[] filebytes = null;
            if (mediaFile != null)
                filebytes = ReadFully(mediaFile.GetStream());

            var donationCapture = new DonationCapture
            {
                Expiration = expirationDate.Add(expirationTime),
                Image = filebytes,
                Amount = quantity,
                Title = donationTitle,
                Type = donationType
            };

            var serializedDonationCapture = JsonConvert.SerializeObject(donationCapture);

            var message = new HttpRequestMessage(HttpMethod.Post, App.BackendUrl + "/api/donations");
            message.Content = new StringContent(serializedDonationCapture, Encoding.UTF8, "application/json");

            var response= await App.ApiClient.SendAsync(message);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CancelDonationAsync(int donationId)
        {
            var response = await App.ApiClient.PutAsync($"api/donations/{donationId}/cancel", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CompleteDonationAsync(int donationId)
        {
            var response = await App.ApiClient.PutAsync($"api/donations/{donationId}/complete", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> WasteDonationAsync(int donationId)
        {
            var response = await App.ApiClient.PutAsync($"api/donations/{donationId}/waste", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RelistDonationAsync(DonationCapture capture, int donationId, MediaFile mediaFile)
        {
            byte[] filebytes = null;
            if (mediaFile != null)
                filebytes = ReadFully(mediaFile.GetStream());

            capture.Image = filebytes;

            var content = new StringContent(JsonConvert.SerializeObject(capture), Encoding.UTF8, "application/json");
            var response = await App.ApiClient.PutAsync($"api/donations/{donationId}/relist", content);
            return response.IsSuccessStatusCode;
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

    }
}
