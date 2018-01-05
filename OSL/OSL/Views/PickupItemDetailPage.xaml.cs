using System;
using OSL.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OSL
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PickupItemDetailPage : ContentPage
    {
        PickupItemDetailViewModel viewModel;

        // Note - The Xamarin.Forms Previewer requires a default, parameterless constructor to render a page.
        public PickupItemDetailPage()
        {
            InitializeComponent();

            var item = new Donation();

            viewModel = new PickupItemDetailViewModel(item);
            BindingContext = viewModel;
        }

        public PickupItemDetailPage(PickupItemDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
            viewModel.Page = this;
        }
    }
}
