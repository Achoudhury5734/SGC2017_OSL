using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;

namespace OSL
{
    public class PickupItemsViewModel : ViewModelBase
    {
        public ObservableCollection<PickupItem> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public Command FilterItemsCommand { get; set; }

        private readonly int[] distances = new int[] { 5, 10, 15 };

        public PickupItemsViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<PickupItem>();
            LoadItemsCommand = new Command(async (range) => await ExecuteLoadItemsCommand((int?)range));
            FilterItemsCommand = new Command(() => ExecuteFilterItemsCommand());

            MessagingCenter.Subscribe<NewPickupItemPage, PickupItem>(this, "AddItem", async (obj, item) =>
            {
                var _item = item as PickupItem;
                Items.Add(_item);
                await DataStore.AddPickupItemAsync(_item);
            });
        }

        // null range for all
        async Task ExecuteLoadItemsCommand(int? range)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                IEnumerable<PickupItem> items;

                if (range == null)
                    items = await DataStore.GetPickupItemsAsync(true);
                else
                    items = await GetItemsWithinRange(range.Value);
                
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

        async Task<IEnumerable<PickupItem>> GetItemsWithinRange(int range)
        {
            var location = await GetCurrentLocation();
            if (location != null)
            {
                return await DataStore.GetFilteredItemsAsync(range, location.Latitude, location.Longitude);
            }
            else
            {
                geolocationFailureToast();
                return await DataStore.GetFilteredItemsAsync(range, null, null);
            }
        }

        void geolocationFailureToast()
        {
            var message = "Unable to get your location. Using organization address instead";
            var toastConfig = new ToastConfig(message);
            toastConfig.Duration = TimeSpan.FromSeconds(10);

            UserDialogs.Instance.Toast(toastConfig);
        }

        void ExecuteFilterItemsCommand()
        {
            var actionConfig = new ActionSheetConfig();
            actionConfig.Title = "Filter Items By Distance";

            foreach (int distance in distances)
            {
                actionConfig.Add(distance + " Miles", (async () => await ExecuteLoadItemsCommand(distance)));
            }
            actionConfig.Add("All", (async () => await ExecuteLoadItemsCommand(null)));

            UserDialogs.Instance.ActionSheet(actionConfig);
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
            catch 
            {
                return null;
            }

            return position;
        }
    }
}
