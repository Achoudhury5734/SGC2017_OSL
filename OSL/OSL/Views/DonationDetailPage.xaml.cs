using OSL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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