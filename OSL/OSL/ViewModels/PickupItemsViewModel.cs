using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
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

        public PickupItemsViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<PickupItem>();
            LoadItemsCommand = new Command(async (range) => await ExecuteLoadItemsCommand((int)range));

            MessagingCenter.Subscribe<NewPickupItemPage, PickupItem>(this, "AddItem", async (obj, item) =>
            {
                var _item = item as PickupItem;
                Items.Add(_item);
                await DataStore.AddPickupItemAsync(_item);
            });
        }

        // -1 range for all
        async Task ExecuteLoadItemsCommand(int range)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                IEnumerable<PickupItem> items;
                if (range == -1)
                {
                    items = await DataStore.GetPickupItemsAsync(true);
                }
                else
                {
                    var location = await GetCurrentLocation();
                    if (location != null)
                        items = await DataStore.GetFilteredItemsAsync(range, location.Latitude, location.Longitude);
                    else
                        items = await DataStore.GetFilteredItemsAsync(range, null, null);
                }
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
                {
                    MessagingCenter.Send(this, "GeolocationFailure");
                    return null;
                }

                position = await locator.GetPositionAsync(TimeSpan.FromSeconds(20), null, true);
            }
            catch 
            {
                MessagingCenter.Send(this, "GeolocationFailure");
                return null;
            }

            if (position == null)
                return null;

            return position;
        }
    }
}
