using System;
using Xamarin.Forms;
using OSL.ViewModels;
using OSL.Models;

namespace OSL.Views
{
    public partial class DonationPage : ContentPage
    {
        public DonationPage()
        {
            InitializeComponent();
            this.BindingContext = new DonationViewModel
            {
                Page = this
            };
        }

        public DonationPage(Donation donation) {
            InitializeComponent();
            this.BindingContext = new RelistViewModel(donation) { Page = this };
        }
    }
}
