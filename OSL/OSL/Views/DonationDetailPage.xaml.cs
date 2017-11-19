using OSL.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OSL.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DonationDetailPage : ContentPage
	{
		public DonationDetailPage (DonationDetailViewModel donationDetailViewModel)
		{
			InitializeComponent ();
            donationDetailViewModel.Page = this;
            this.BindingContext = donationDetailViewModel;

        }
    }
}