﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using OSL.Models;

namespace OSL.Views
{
    public partial class MainPage : ContentPage
    {
        private bool loggingOut = false;

        public MainPage(bool logout = false)
        {
            loggingOut = logout;

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
                App.User = GetUser(ar);

                Application.Current.MainPage = new RootPage();
            }
            catch (Exception ex)
            {
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
                AuthenticationResult ar = await App.PCA.AcquireTokenAsync(App.Scopes, GetUserByPolicy(App.PCA.Users, App.PolicySignUpSignIn), App.UiParent);
                App.AccessToken = ar.AccessToken;
                App.User = GetUser(ar);

                UpdateButtonState(true);

                if (App.User == null) {
                    Application.Current.MainPage = new RegisterPage();
                } else {
                    Application.Current.MainPage = new RootPage();
                }
            }
            catch (Exception ex)
            {
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

        public User GetUser(AuthenticationResult ar)
        {
            JObject user = ParseIdToken(ar.IdToken);
            if (user["newUser"] != null && user["newUser"].ToString() == "true") {
                return null;
            }

            return new User();
        }

        JObject ParseIdToken(string idToken)
        {
            // Get the piece with actual user info
            idToken = idToken.Split('.')[1];
            idToken = Base64UrlDecode(idToken);
            return JObject.Parse(idToken);
        }
    }
}
