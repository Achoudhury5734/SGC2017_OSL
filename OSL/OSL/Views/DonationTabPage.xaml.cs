using System;
using System.Collections.Generic;
using OSL.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OSL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DonationTabPage : TabbedPage
    {
        public DonationTabPage()
        {
            InitializeComponent();
            this.Title = "My Donations";

            var listed = new DonationListPage(DonationStatus.Listed);
            listed.Title = "Listed";

            var pending = new DonationListPage(DonationStatus.PendingPickup);
            pending.Title = "Pending";

            var completed = new DonationListPage(DonationStatus.Completed);
            completed.Title = "Completed";

            var wasted = new DonationListPage(DonationStatus.Wasted);
            wasted.Title = "Wasted";

#if __IOS__
            listed.Icon = "icons8_list";
            pending.Icon = "icons8_clock";
            completed.Icon = "icons8_ok";
            wasted.Icon = "icons8_trash";
#endif

            this.Children.Add(listed);
            this.Children.Add(pending);
            this.Children.Add(completed);
            this.Children.Add(wasted);
        }
    }
}
