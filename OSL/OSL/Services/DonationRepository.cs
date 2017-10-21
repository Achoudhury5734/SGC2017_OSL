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
        static DonationRepository()
        {
            Donations = new List<Donation>();
        }

        public static List<Donation> Donations { get; set; }

        public Task<Donation> GetDonationAsync(int donationId)
        {
            return Task.FromResult(Donations.Find(d => d.Id == donationId));
        }

        public async Task<IEnumerable<Donation>> GetDonationsByUserAsync()
        {
            //var json = await App.ApiClient.GetStringAsync("api/donations/donor/me");
            var json = await App.ApiClient.GetStringAsync("api/donations");

            return JsonConvert.DeserializeObject<IEnumerable<Donation>>(json);
        }

        public async Task SaveDonationAsync(string donationTitle, MediaFile mediaFile, int quantity, string donationType, DateTime expirationDate, TimeSpan expirationTime)
        {
            var filebytes = ReadFully(mediaFile.GetStream());
            var donationCapture = new DonationCapture
            {
                Expiration = expirationDate.Add(expirationTime),
                Image = JsonConvert.SerializeObject(filebytes),
                Amount = quantity,
                Title = donationTitle,
                Type = donationType
            };

            var serializedDonationCapture = JsonConvert.SerializeObject(donationCapture);

            var message = new HttpRequestMessage(HttpMethod.Post, App.BackendUrl + "/api/donations");
            message.Content = new StringContent(serializedDonationCapture, Encoding.UTF8, "application/json");

            var response = await App.ApiClient.SendAsync(message);
            response.EnsureSuccessStatusCode();
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
