using System;
using OSL.Models;
using OSL.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OSL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AcceptedItemsPage : ContentPage
    {
        AcceptedItemsViewModel viewModel;

        public AcceptedItemsPage()
        {
            InitializeComponent();
        }

        public AcceptedItemsPage(DonationStatus status)
        {
            InitializeComponent();
            viewModel = new AcceptedItemsViewModel(status);
            this.BindingContext = viewModel;
            MessagingCenter.Subscribe<AcceptedDetailViewModel, Donation>(this, "StatusChanged", OnItemCancelled);
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Donation;
            if (item == null)
                return;

            await Navigation.PushAsync(new AcceptedDetailPage(new AcceptedDetailViewModel(item)));

            // Manually deselect item
            AcceptedItemsListView.SelectedItem = null;
        }

        private void OnItemCancelled(AcceptedDetailViewModel sender, Donation item) {
            viewModel.Items.Remove(item);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }
    }
}
