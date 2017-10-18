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
		public DonationListPage()
		{
			InitializeComponent();
            this.BindingContext = new DonationListViewModel
            {
                Page = this
            };
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
    }
}