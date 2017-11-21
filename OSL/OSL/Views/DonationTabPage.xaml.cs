using System;
using System.Collections.Generic;
using OSL.Models;

using Xamarin.Forms;

namespace OSL.Views
{
    public partial class DonationTabPage : TabbedPage
    {
        public DonationTabPage()
        {
            InitializeComponent();
            this.Title = "My Donations";

            var listed = new DonationListPage(DonationStatus.Listed);
            listed.Title = "Listed";
            listed.Icon = null;

            var pending = new DonationListPage(DonationStatus.PendingPickup);
            pending.Title = "Pending";

            var completed = new DonationListPage(DonationStatus.Completed);
            completed.Title = "Completed";

            var wasted = new DonationListPage(DonationStatus.Wasted);
            wasted.Title = "Wasted";

            this.Children.Add(listed);
            this.Children.Add(pending);
            this.Children.Add(completed);
            this.Children.Add(wasted);
        }
    }
}
