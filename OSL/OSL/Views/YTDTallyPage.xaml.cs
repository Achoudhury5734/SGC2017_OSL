using System;
using OSL.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OSL.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class YTDTallyPage : ContentPage
	{
        YTDViewModel viewModel;

        public YTDTallyPage()
        {
            InitializeComponent();
            viewModel = new YTDViewModel();
            BindingContext = viewModel;
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.LoadAmountsCommand.Execute(null);
        }
	}
}