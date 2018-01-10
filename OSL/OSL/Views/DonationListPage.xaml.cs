using OSL;
using OSL.Models;
using OSL.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OSL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DonationListPage : ContentPage
	{
        DonationListViewModel viewModel;

		public DonationListPage()
		{
			InitializeComponent();
        }

        public DonationListPage(DonationStatus status)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new DonationListViewModel(status)
            {
                Page = this
            };
            MessagingCenter.Subscribe<DonationDetailViewModel, Donation>(this, "StatusChanged", OnStatusChanged);
        }

        private void OnStatusChanged(DonationDetailViewModel sender, Donation item) {
            viewModel.Items.Remove(item);
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Donation;
            if (item == null)
                return;

            await Navigation.PushAsync(new DonationDetailPage(new DonationDetailViewModel(item)));

            // Manually deselect item
            ItemsListView.SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }
    }
}