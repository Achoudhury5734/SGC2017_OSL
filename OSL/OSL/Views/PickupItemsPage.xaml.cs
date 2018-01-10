using System;
using OSL.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OSL
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PickupItemsPage : ContentPage
    {
        PickupItemsViewModel viewModel;

        public PickupItemsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new PickupItemsViewModel();
            MessagingCenter.Subscribe<PickupItemDetailViewModel,Donation>(this, "ItemAccepted", OnItemAccepted);
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Donation;
            if (item == null)
                return;

            await Navigation.PushAsync(new PickupItemDetailPage(new PickupItemDetailViewModel(item)));

            // Manually deselect item
            PickupItemsListView.SelectedItem = null;
        }

        private void OnItemAccepted(PickupItemDetailViewModel sender, Donation item) {
            viewModel.Items.Remove(item);
        }

        public void OnTextChanged(object s, EventArgs e)
        {
            var bar = s as SearchBar;
            if (String.IsNullOrWhiteSpace(bar.Text))
            {
                viewModel.SearchCommand.Execute(null);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }
    }
}
