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

        public DonationListViewModel()
        {
            this.donationRepository = new DonationRepository();
            Title = "My Donations";
            Items = new ObservableCollection<Group>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsAsync(), () => !IsBusy);
        }

        public ObservableCollection<Group> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        async Task ExecuteLoadItemsAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var pending = new Group("Pending Pickup");
                var completed = new Group("Completed");
                var wasted = new Group("Wasted");
                var listed = new Group("Listed");
                Items.Clear();
                var items = await donationRepository.GetDonationsByUserAsync();

                foreach (var item in items)
                {
                    if (item.Status == DonationStatus.PendingPickup)
                        pending.Add(item);
                    else if (item.Status == DonationStatus.Completed)
                        completed.Add(item);
                    else if (item.Status == DonationStatus.Wasted)
                        wasted.Add(item);
                    else
                        listed.Add(item);
                }
                Items.Add(pending);
                Items.Add(listed);
                Items.Add(completed);
                Items.Add(wasted);
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

        public class Group : ObservableCollection<Donation>
        {
            public string Key { get; set; }

            public Group(string key)
            {
                Key = key;
            }
        }
    }
}
