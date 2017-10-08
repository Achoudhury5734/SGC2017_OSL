using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace OSL.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            //Navigation.PushModalAsync(new NewItemPage());
            /*Page itemsPage, aboutPage = null;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    itemsPage = new NavigationPage(new ItemsPage())
                    {
                        Title = "Browse"
                    };

                    aboutPage = new NavigationPage(new AboutPage())
                    {
                        Title = "About"
                    };
                    itemsPage.Icon = "tab_feed.png";
                    aboutPage.Icon = "tab_about.png";
                    break;
                default:
                    itemsPage = new ItemsPage()
                    {
                        Title = "Browse"
                    };

                    aboutPage = new AboutPage()
                    {
                        Title = "About"
                    };
                    break;
            }

            Children.Add(itemsPage);
            Children.Add(aboutPage);

            Title = Children[0].Title;*/
        }

        protected override async void OnAppearing()
        {
            await UpdateSignInStateAsync(false);

            // Check to see if we have a User
            // in the cache already.
            try
            {
                AuthenticationResult ar = await App.PCA.AcquireTokenSilentAsync(App.Scopes, GetUserByPolicy(App.PCA.Users, App.PolicySignUpSignIn), App.Authority, false);
                UpdateUserInfo(ar);
                await UpdateSignInStateAsync(true);
            }
            catch (Exception ex)
            {
                // Uncomment for debugging purposes
                //await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");

                // Doesn't matter, we go in interactive mode
                UpdateSignInStateAsync(false).Wait();
            }
        }

        void OnClickRegister(object sender, EventArgs e)
        {
            DisplayAlert("1","2","3").Wait();
        }

        async void OnSignInSignOut(object sender, EventArgs e)
        {
            try
            {
                if (btnSignInSignOut.Text == "Sign in")
                {
                    AuthenticationResult ar = await App.PCA.AcquireTokenAsync(App.Scopes, GetUserByPolicy(App.PCA.Users, App.PolicySignUpSignIn), App.UiParent);
                    UpdateUserInfo(ar);
                    await UpdateSignInStateAsync(true);
                }
                else
                {
                    foreach (var user in App.PCA.Users)
                    {
                        App.PCA.Remove(user);
                    }
                    await UpdateSignInStateAsync(false);
                }
            }
            catch (Exception ex)
            {
                // Checking the exception message 
                // should ONLY be done for B2C
                // reset and not any other error.
                if (ex.Message.Contains("AADB2C90118"))
                    OnPasswordReset();
                // Alert if any exception excludig user cancelling sign-in dialog
                else if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }

        private IUser GetUserByPolicy(IEnumerable<IUser> users, string policy)
        {
            foreach (var user in users)
            {
                string userIdentifier = Base64UrlDecode(user.Identifier.Split('.')[0]);
                if (userIdentifier.EndsWith(policy.ToLower())) return user;
            }

            return null;
        }

        private string Base64UrlDecode(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/');
            s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
            var byteArray = Convert.FromBase64String(s);
            var decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
            return decoded;
        }

        public void UpdateUserInfo(AuthenticationResult ar)
        {
            JObject user = ParseIdToken(ar.IdToken);
            lblName.Text = user["emails"]?[0]?.ToString();
            lblId.Text = user["oid"]?.ToString();
        }

        JObject ParseIdToken(string idToken)
        {
            // Get the piece with actual user info
            idToken = idToken.Split('.')[1];
            idToken = Base64UrlDecode(idToken);
            return JObject.Parse(idToken);
        }

        async void OnCallApi(object sender, EventArgs e)
        {
            try
            {
                lblApi.Text = $"Calling API {App.BackendUrl}";

                AuthenticationResult ar = await App.PCA.AcquireTokenSilentAsync(App.Scopes, GetUserByPolicy(App.PCA.Users, App.PolicySignUpSignIn), App.Authority, false);
                string token = ar.AccessToken;

                // Get data from API
                HttpClient client = new HttpClient();
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, App.BackendUrl);
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.SendAsync(message);
                string responseString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    lblApi.Text = $"Response from API {App.BackendUrl} | {responseString}";
                }
                else
                {
                    lblApi.Text = $"Error calling API {App.BackendUrl} | {responseString}";
                }
            }
            catch (MsalUiRequiredException ex)
            {
                await DisplayAlert($"Session has expired, please sign out and back in.", ex.ToString(), "Dismiss");
            }
            catch (Exception ex)
            {
                await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }

        async void OnPasswordReset()
        {
            try
            {
                AuthenticationResult ar = await App.PCA.AcquireTokenAsync(App.Scopes, (IUser)null, UIBehavior.SelectAccount, string.Empty, null, App.AuthorityPasswordReset, App.UiParent);
                UpdateUserInfo(ar);
            }
            catch (Exception ex)
            {
                // Alert if any exception excludig user cancelling sign-in dialog
                if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }

        async Task UpdateSignInStateAsync(bool isSignedIn)
        {
            btnSignInSignOut.Text = isSignedIn ? "Sign out" : "Sign in";
            //btnPasswordReset.IsVisible = isSignedIn;
            btnCallApi.IsVisible = isSignedIn;
            slUser.IsVisible = isSignedIn;
            lblApi.Text = "";
            await App.NavigationPage.Navigation.PushAsync(new PickupItemsPage());
        }
    }
}
