using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;

namespace OSL
{
    public class PickupItemsViewModel : ViewModelBase
    {
        public ObservableCollection<PickupItem> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public Command LoadFilteredItemsCommand { get; set; }

        public PickupItemsViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<PickupItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            LoadFilteredItemsCommand = new Command(async () => await ExecuteFilteredItemsCommand());

            MessagingCenter.Subscribe<NewPickupItemPage, PickupItem>(this, "AddItem", async (obj, item) =>
            {
                var _item = item as PickupItem;
                Items.Add(_item);
                await DataStore.AddPickupItemAsync(_item);
            });

            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 100;
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

        async Task<Position> GetCurrentLocation()
        {
            Position position = null;
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 100;

                position = await locator.GetLastKnownLocationAsync();

                if (position != null)
                    return position;

                if (!locator.IsGeolocationAvailable || !locator.IsGeolocationEnabled)
                    return null;

                position = await locator.GetPositionAsync(TimeSpan.FromSeconds(20), null, true);
            }
            catch (Exception ex)
            {

            }

            if (position == null)
                return null;

            return position;
        }

        async Task ExecuteFilteredItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var location = await GetCurrentLocation();
                var items = await DataStore.GetFilteredItemsAsync(location.Latitude, location.Longitude);
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
