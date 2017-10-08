using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace OSL
{
    public partial class PickupItemsPage : ContentPage
    {
        PickupItemsViewModel viewModel;

        public PickupItemsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new PickupItemsViewModel();
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

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewPickupItemPage());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }
    }
}
