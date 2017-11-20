using System;
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
        public ObservableCollection<Donation> Items { get; set; }

        private readonly DonationRepository donationRep;

        public AcceptedItemsViewModel(DonationStatus status)
        {
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand(status));
            donationRep = new DonationRepository();
            Items = new ObservableCollection<Donation>();
            string statusString = status == DonationStatus.PendingPickup ? "Pending Pickup" : status.ToString("G");
            Title = statusString;
        }

        async Task ExecuteLoadItemsCommand(DonationStatus status)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await donationRep.GetDonationsByRecipientAsync(status);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
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
    }
}
