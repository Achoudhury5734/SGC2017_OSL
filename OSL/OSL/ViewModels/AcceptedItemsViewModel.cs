using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using OSL.Models;
using OSL.Services;
using Xamarin.Forms;

namespace OSL.ViewModels
{
    public class AcceptedItemsViewModel : ViewModelBase
    {
        public Command LoadItemsCommand { get; set; }
        public ObservableCollection<Group> GroupedItems { get; set; }

        private readonly DonationRepository donationRep;

        public AcceptedItemsViewModel()
        {
            LoadItemsCommand = new Command(async() => await ExecuteLoadItemsCommand());
            donationRep = new DonationRepository();
            GroupedItems = new ObservableCollection<Group>();
            Title = "Accepted Donations";
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                GroupedItems.Clear();
                var items = await donationRep.GetDonationsByRecipientAsync();
                var pending = new Group("Pending Pickup");
                var completed = new Group("Completed");
                var wasted = new Group("Wasted");
                foreach (var item in items)
                {
                    if (item.Status == DonationStatus.PendingPickup)
                        pending.Add(item);
                    if (item.Status == DonationStatus.Completed)
                        completed.Add(item);
                    if (item.Status == DonationStatus.Wasted)
                        wasted.Add(item);
                }
                GroupedItems.Add(pending);
                GroupedItems.Add(completed);
                GroupedItems.Add(wasted);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            } 
        }

        public class Group: ObservableCollection<Donation>
        {
            public string Key { get; set; }

            public Group (string key)
            {
                Key = key;
            }
        }
    }
}
