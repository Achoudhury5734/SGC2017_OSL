using System;
using Plugin.Media;
using Xamarin.Forms;
using OSL.ViewModels;

namespace OSL.Views
{
    public partial class DonationPage : ContentPage
    {
        private int? donationId;
        private RelistViewModel viewModel;

        public DonationPage()
        {
            InitializeComponent();
            this.BindingContext = new DonationViewModel
            {
                Page = this
            };
        }

        public DonationPage(int id)
        {
            InitializeComponent();
            donationId = id;
            viewModel = new RelistViewModel(id) {Page = this};
            this.BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (donationId.HasValue)
                viewModel.LoadDonationCommand.Execute(donationId.Value);
        }
    }
}
