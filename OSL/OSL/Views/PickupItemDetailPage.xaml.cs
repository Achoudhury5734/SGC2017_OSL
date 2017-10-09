using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OSL
{
    public partial class PickupItemDetailPage : ContentPage
    {
        PickupItemDetailViewModel viewModel;
        CloudDataStore dataStore;

        // Note - The Xamarin.Forms Previewer requires a default, parameterless constructor to render a page.
        public PickupItemDetailPage()
        {
            InitializeComponent();

            dataStore = new CloudDataStore();
            var item = new PickupItem
            {
                Title = "New Item posted"
            };

            viewModel = new PickupItemDetailViewModel(item);
            BindingContext = viewModel;
        }

        public PickupItemDetailPage(PickupItemDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;

            Accept.Clicked += Accept_Clicked;
        }

        private async void Accept_Clicked(object sender, EventArgs e)
        {
            var res = await dataStore.AcceptPickupItemAsync(viewModel.Item);

            await DisplayAlert("Information", "Thank you for accepting the donation", "OK");
                        
            await this.Navigation.PopAsync();
        }
    }
}
