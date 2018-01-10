using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using OSL.Helpers;

using Xamarin.Forms;
using Microsoft.Identity.Client;
using OSL.Views;
using OSL.Models;
using System.Net.Http;
using System;
using Xamarin.Forms.Xaml;

namespace OSL
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class App : Application
    {
        public static PublicClientApplication PCA = null;

        // Azure AD B2C Coordinates
        public static string Tenant = "secondhelping.onmicrosoft.com";
        public static string ClientID = "1771869e-f3aa-4d16-bd1a-af2f0e662bcd";
        public static string PolicySignUpSignIn = "B2C_1_SignUpSignIn";
        public static string PolicyResetPassword = "B2C_1_ProfileEdit";

        public static string[] Scopes = { "https://secondhelping.onmicrosoft.com/shapi/user" };

        public static string AuthorityBase = $"https://login.microsoftonline.com/tfp/{Tenant}/";
        public static string Authority = $"{AuthorityBase}{PolicySignUpSignIn}";
        public static string AuthorityPasswordReset = $"{AuthorityBase}{PolicyResetPassword}";

        public static bool UseMockDataStore = false;
        public static string BackendUrl = "https://secondhelpingweb.azurewebsites.net";

        public static UIParent UiParent = null;

        public static HttpClient ApiClient;

        public static User User { get; set; }
        public static string AccessToken { get; set; }

        public App()
        {
            InitializeComponent();

            PCA = new PublicClientApplication(ClientID, Authority);
            PCA.RedirectUri = $"msal{ClientID}://auth";

#if !DEBUG
            MobileCenter.Start($"android={Constants.MobileCenterAndroid};" +
                   $"uwp={Constants.MobileCenterUWP};" +
                   $"ios={Constants.MobileCenteriOS}",
                   typeof(Analytics), typeof(Crashes));
#endif

            ApiClient = new HttpClient();
            ApiClient.BaseAddress = new Uri($"{BackendUrl}/");

            if (UseMockDataStore)
                DependencyService.Register<MockDataStore>();
            else
                DependencyService.Register<CloudDataStore>();

            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
                MainPage = new MainPage();
            else
                MainPage = new NavigationPage(new MainPage());
        }
    }
}
