using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Microsoft.Identity.Client;
using Plugin.Permissions;
using Xamarin.Forms;

namespace OSL.Droid
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/osl_icon", 
        Theme = "@style/MyTheme", MainLauncher = false, 
        LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();
            UserDialogs.Init(() => (Activity)Forms.Context);

            App.UiParent = new UIParent(Xamarin.Forms.Forms.Context as Activity);

            LoadApplication(new App());
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
