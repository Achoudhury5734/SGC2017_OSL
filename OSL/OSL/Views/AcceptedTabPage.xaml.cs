using System;
using System.Collections.Generic;
using OSL.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OSL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AcceptedTabPage : TabbedPage
    {
        public AcceptedTabPage()
        {
            InitializeComponent();
            this.Title = "Accepted Donations";

            var pending = new AcceptedItemsPage(DonationStatus.PendingPickup);
            pending.Title = "Pending";

            var completed = new AcceptedItemsPage(DonationStatus.Completed);
            completed.Title = "Completed";

#if __IOS__
            completed.Icon = "icons8_ok";
            pending.Icon = "icons8_clock";
#endif

            this.Children.Add(pending);
            this.Children.Add(completed);
        }
    }
}
