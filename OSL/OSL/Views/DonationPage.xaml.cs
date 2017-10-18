using System;
using Plugin.Media;
using Xamarin.Forms;
using OSL.ViewModels;

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
    }
}
