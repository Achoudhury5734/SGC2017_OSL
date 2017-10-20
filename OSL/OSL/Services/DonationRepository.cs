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

        public Task<IEnumerable<Donation>> GetDonationsByUserAsync()
        {
            return Task.FromResult(Donations.AsEnumerable());
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
            try
            {
                var message = new HttpRequestMessage(HttpMethod.Post, App.BackendUrl + "/api/donations");
                message.Content = new StringContent(serializedDonationCapture, Encoding.UTF8, "application/json");

                var response = await App.ApiClient.SendAsync(message);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {

                throw;
            }

            //Just for testing...
            Donations.Add(new Donation { Title = donationCapture.Title, PictureUrl = "https://i.ytimg.com/vi/jqDUmYVQxOI/hqdefault.jpg", Type = (DonationType)Enum.Parse(typeof(DonationType), donationCapture.Type ) });
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
