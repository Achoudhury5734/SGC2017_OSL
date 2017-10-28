using OSL.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;
using System.Text.RegularExpressions;

namespace OSL
{
    public partial class PickupItemsPage : ContentPage
    {
        PickupItemsViewModel viewModel;

        public PickupItemsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new PickupItemsViewModel();
            MessagingCenter.Subscribe<PickupItemsViewModel>(this, "GeolocationFailure", GeolocationFailed);
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

        void Filter_Changed(object sender, EventArgs e)
        {
            int range = -1;
            var selected = Regex.Replace(DistancePicker.SelectedItem.ToString(), "[^0-9.]", "");
            if (selected != "")
            {
                range = int.Parse(selected);
            }
            viewModel.LoadItemsCommand.Execute(range);
        }

        private void GeolocationFailed(PickupItemsViewModel obj)
        {
            DisplayAlert("Geolocation Failed", "Using organization address instead", "Ok");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(-1);
        }
    }
}
