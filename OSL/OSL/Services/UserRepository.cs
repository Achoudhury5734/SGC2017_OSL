using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OSL.Models;

namespace OSL.Services
{
    public class UserRepository
    {
        public async Task<User> GetUserFromIdentityToken(string identityToken) {
            JObject arUser = ParseIdToken(identityToken);

            try
            {
                var json = await App.ApiClient.GetStringAsync($"api/users/me");
                var user = JsonConvert.DeserializeObject<User>(json);
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching user: " + ex.StackTrace);
                return null;
            }
        }

        private JObject ParseIdToken(string idToken)
        {
            // Get the piece with actual user info
            idToken = idToken.Split('.')[1];
            idToken = Base64UrlDecode(idToken);
            return JObject.Parse(idToken);
        }

        private string Base64UrlDecode(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/');
            s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
            var byteArray = Convert.FromBase64String(s);
            var decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
            return decoded;
        }

        public async Task<User> Create(User user)
        {
            var response = await App.ApiClient.PostAsync($"api/users", new StringContent(JsonConvert.SerializeObject(user)));

            if (response.StatusCode == HttpStatusCode.OK) {
                return user;
            }

            return null;
        }

        public async Task<User> Update(User user)
        {
            var response = await App.ApiClient.PutAsync($"api/users/me", new StringContent(JsonConvert.SerializeObject(user)));

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return user;
            }

            return null;
        }
    }
}
