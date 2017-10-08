using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace OSL
{
    public class PickupItemsViewModel : ViewModelBase
    {
        public ObservableCollection<PickupItem> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public PickupItemsViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<PickupItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<NewPickupItemPage, PickupItem>(this, "AddItem", async (obj, item) =>
            {
                var _item = item as PickupItem;
                Items.Add(_item);
                await DataStore.AddPickupItemAsync(_item);
            });
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetPickupItemsAsync(true);
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
