using System;
using System.Collections.Generic;
using OSL.ViewModels;
using Xamarin.Forms;

namespace OSL.Views
{
    public partial class AcceptedDetailPage : ContentPage
    {
        AcceptedDetailViewModel viewModel;

        public AcceptedDetailPage()
        {
            InitializeComponent();
        }

        public AcceptedDetailPage(AcceptedDetailViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            viewModel.Page = this;
            this.BindingContext = viewModel;
        }
    }
}
