using System;
using System.Collections.Generic;
using Microsoft.Identity.Client;
using Xamarin.Forms;
using System.Net.Http.Headers;
using OSL.Services;
using System.Linq;
using System.Text;
using Xamarin.Forms.Xaml;

namespace OSL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        private bool loggingOut;
        private UserRepository userRepository;

        public MainPage(bool logout = false)
        {
            loggingOut = logout;
            userRepository = new UserRepository();

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            if (loggingOut) {
                OnClickLogout(null, null);
                return;
            }

            try
            {
                UpdateButtonState(true);

                // Check to see if we have a User in the cache already.
                AuthenticationResult ar = await App.PCA.AcquireTokenSilentAsync(App.Scopes, GetUserByPolicy(App.PCA.Users, App.PolicySignUpSignIn), App.Authority, false);
                App.AccessToken = ar.AccessToken;
                App.ApiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ar.AccessToken);
                App.User = await userRepository.GetUserFromIdentityToken(ar.IdToken);

                if (App.User == null)
                {
                    Application.Current.MainPage = new RegisterPage();
                }
                else
                {
                    Application.Current.MainPage = new RootPage();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading user: " + ex.StackTrace);
                UpdateButtonState(false);
            }
        }

        private void UpdateButtonState(bool isSignedIn) {
            btnLogout.IsVisible = isSignedIn;
            btnLogin.IsVisible = !isSignedIn;
            btnSignup.IsVisible = !isSignedIn;
        }

        async void OnClickSignup(object sender, EventArgs e)
        {
            try
            {
                UpdateButtonState(true);

                AuthenticationResult ar = await App.PCA.AcquireTokenAsync(App.Scopes, GetUserByPolicy(App.PCA.Users, App.PolicySignUpSignIn), App.UiParent);
                App.AccessToken = ar.AccessToken;
                App.ApiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ar.AccessToken);
                App.User = await userRepository.GetUserFromIdentityToken(ar.IdToken);

                if (App.User == null) {
                    Application.Current.MainPage = new RegisterPage();
                } else {
                    Application.Current.MainPage = new RootPage();
                }
            }
            catch (Exception ex)
            {
                UpdateButtonState(false);
                if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                {
                    // Alert if any exception excludig user cancelling sign-in dialog
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
                }
            }
        }

        void OnClickLogout(object sender, EventArgs e) {
            foreach (var user in App.PCA.Users)
            {
                App.PCA.Remove(user);
            }

            UpdateButtonState(false);
        }

        private IUser GetUserByPolicy(IEnumerable<IUser> users, string policy)
        {
            foreach (var user in users)
            {
                string userIdentifier = Base64UrlDecode(user.Identifier.Split('.')[0]);
                if (userIdentifier.EndsWith(policy.ToLower(), StringComparison.CurrentCulture)) return user;
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
    }
}
