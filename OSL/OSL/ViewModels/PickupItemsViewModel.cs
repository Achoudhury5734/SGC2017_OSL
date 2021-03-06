﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using OSL.Models;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;

namespace OSL
{
    public class PickupItemsViewModel : ViewModelBase
    {
        public ObservableCollection<Donation> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public Command FilterItemsCommand { get; set; }
        public Command SearchCommand { get; set; }
        public bool SearchEnabled { get; set; }
        public Command EnableSearchCommand { get; set; } 
        public string Text { get; set; }
        public string ToolbarText { get; set; }

        private readonly int[] distances = { 5, 10, 15 };
        private ICollection<Donation> allItems;

        public PickupItemsViewModel()
        {
            Items = new ObservableCollection<Donation>();
            LoadItemsCommand = new Command(async (range) => await ExecuteLoadItemsCommand((int?)range));
            FilterItemsCommand = new Command(ExecuteFilterItemsCommand);
            EnableSearchCommand = new Command(EnableSearch);
            SearchCommand = new Command(ExecuteSearchCommand);
            SearchEnabled = false;
            allItems = new List<Donation>();
            ToolbarText = "Search";

            MessagingCenter.Subscribe<PickupItemDetailViewModel, Donation>(this, "ItemAccepted", OnItemAccepted);
        }

        // Remove item from future search results
        private void OnItemAccepted(PickupItemDetailViewModel sender, Donation item) {
            allItems.Remove(item);
        }

        private void EnableSearch()
        {
            SearchEnabled = !SearchEnabled;
            OnPropertyChanged("SearchEnabled");
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
                IEnumerable<Donation> items;

                if (range == null)
                    items = await DataStore.GetPickupItemsAsync(true);
                else
                    items = await GetItemsWithinRange(range.Value);
                
                foreach (var item in items)
                {
                    Items.Add(item);
                }
                allItems = Items.ToList();
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

        void ExecuteSearchCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                if (!String.IsNullOrEmpty(Text))
                {
                    var searched = Text.ToLower();
                    var items = allItems.Where(item => item.Title.ToLower().Contains(searched) ||
                                        item.Donor.Person_Name.ToLower().Contains(searched) ||
                                        item.Donor.Organization_Name.ToLower().Contains(searched));

                    foreach (var item in items)
                    {
                        Items.Add(item);
                    }
                } else {
                    foreach (var item in allItems) 
                    {
                        Items.Add(item);
                    }
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

        async Task<IEnumerable<Donation>> GetItemsWithinRange(int range)
        {
            var location = await GetCurrentLocation();
            if (location != null)
            {
                return await DataStore.GetFilteredItemsAsync(range, location.Latitude, location.Longitude);
            }
            else
            {
                GeolocationFailureToast();
                return await DataStore.GetFilteredItemsAsync(range, null, null);
            }
        }

        void GeolocationFailureToast()
        {
            var message = "Unable to get your current location.";
            var toastConfig = new ToastConfig(message);
            toastConfig.Duration = TimeSpan.FromSeconds(10);

            UserDialogs.Instance.Toast(toastConfig);
        }

        void ExecuteFilterItemsCommand()
        {
            var actionConfig = new ActionSheetConfig();
            actionConfig.Title = "Distance:";

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
