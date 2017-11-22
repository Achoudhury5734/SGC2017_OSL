using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace OSL
{
    public class AboutViewModel : ViewModelBase
    {
        public string About { get; } = "We are a non profit on a mission to bring food that is nutritional and safe to all those who struggle with hunger in our community." +
            " We do this through the creation and delivery of nutritionally dense non-cost meals and by being instrumental in advocating for, and supporting, an equitable food system for all. \n\n" +
            "Help us continue our record breaking service in " + DateTime.Now.Year + "! \n\n" +
            
          "For more information about our projects and events visit our website: https://www.oslserves.org/";
        public string Version { get; } = $"Version: {Plugin.VersionTracking.CrossVersionTracking.Current.CurrentVersion}";
        public string Copyright { get; } = $"Copyright {DateTime.Now.Year} Operation Sack Lunch";
        public AboutViewModel()
        {
            Title = "";

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://www.oslserves.org/")));
            LinkCommand = new Command(() => Device.OpenUri(new Uri("https://icons8.com")));
        }

        public ICommand OpenWebCommand { get; }
        public Command LinkCommand { get;}
    }
}
