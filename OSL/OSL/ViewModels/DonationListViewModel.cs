using OSL.Models;
using OSL.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace OSL
{
    public class DonationListViewModel : ViewModelBase
    {
        private readonly DonationRepository donationRepository;

        public DonationListViewModel(DonationStatus status)
        {
            this.donationRepository = new DonationRepository();
            Title = (status == DonationStatus.PendingPickup? "Pending " : status.ToString("G")) + " Donations";
            Items = new ObservableCollection<Donation>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsAsync(status), () => !IsBusy);
        }

        public ObservableCollection<Donation> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        async Task ExecuteLoadItemsAsync(DonationStatus status)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await donationRepository.GetDonationsByDonorAsync(status);

                foreach (var item in items) {
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

        private Page page;
        public Page Page
        {
            get { return page; }
            set { SetProperty(ref page, value); }
        }
    }
}
