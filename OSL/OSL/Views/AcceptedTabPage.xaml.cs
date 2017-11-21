using System;
using System.Collections.Generic;
using OSL.Models;

using Xamarin.Forms;

namespace OSL.Views
{
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

            this.Children.Add(pending);
            this.Children.Add(completed);
        }
    }
}
