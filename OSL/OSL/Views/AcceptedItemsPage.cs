using System;
using System.Collections.Generic;
using OSL.Models;
using OSL.ViewModels;
using Xamarin.Forms;

namespace OSL.Views
{
    public partial class AcceptedItemsPage : ContentPage
    {
        AcceptedItemsViewModel viewModel;

        public AcceptedItemsPage()
        {
            InitializeComponent();
            viewModel = new AcceptedItemsViewModel();
            this.BindingContext = viewModel;
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.LoadItemsCommand.Execute(null);
        }
    }
}
