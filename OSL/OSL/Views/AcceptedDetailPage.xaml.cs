using System;
using OSL.ViewModels;
using Xamarin.Forms;

namespace OSL.Views
{
    public partial class AcceptedDetailPage : ContentPage
    {
        public AcceptedDetailPage()
        {
            InitializeComponent();
        }

        public AcceptedDetailPage(AcceptedDetailViewModel viewModel)
        {
            InitializeComponent();
            viewModel.Page = this;
            this.BindingContext = viewModel;
        }
    }
}
