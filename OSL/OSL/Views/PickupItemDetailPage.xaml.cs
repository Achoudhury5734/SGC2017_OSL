using System;

using Xamarin.Forms;

namespace OSL
{
    public partial class PickupItemDetailPage : ContentPage
    {
        PickupItemDetailViewModel viewModel;

        // Note - The Xamarin.Forms Previewer requires a default, parameterless constructor to render a page.
        public PickupItemDetailPage()
        {
            InitializeComponent();

            var item = new PickupItem
            {
                Text = "Item 1",
                Description = "This is an item description."
            };

            viewModel = new PickupItemDetailViewModel(item);
            BindingContext = viewModel;
        }

        public PickupItemDetailPage(PickupItemDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }
    }
}
