using OSL.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;
using System.Text.RegularExpressions;
using Plugin.Toasts;

namespace OSL
{
    public partial class PickupItemsPage : ContentPage
    {
        PickupItemsViewModel viewModel;

        public PickupItemsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new PickupItemsViewModel();
            MessagingCenter.Subscribe<PickupItemsViewModel>(this, "GeolocationFailure", async (obj) =>
            {
                var notificator = DependencyService.Get<IToastNotificator>();

                var options = new NotificationOptions()
                {
                    Title = "Geolocation Failed",
                    Description = "Using organization address instead",
                    ClearFromHistory = true
                };

                var result = await notificator.Notify(options);
            });
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as PickupItem;
            if (item == null)
                return;

            await Navigation.PushAsync(new PickupItemDetailPage(new PickupItemDetailViewModel(item)));

            // Manually deselect item
            PickupItemsListView.SelectedItem = null;
        }

       void FilterChanged(object sender, EventArgs e)
        {
            int range = -1;
            var picker = sender as Picker;
            var selected = Regex.Replace(picker.SelectedItem.ToString(), "[^0-9.]", "");
            if (selected != "")
            {
                range = int.Parse(selected);
            }
            viewModel.LoadItemsCommand.Execute(range);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(-1);
        }
    }
}
