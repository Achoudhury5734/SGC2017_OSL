using System;
using OSL.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OSL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
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
