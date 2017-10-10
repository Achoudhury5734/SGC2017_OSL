using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSL.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OSL.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class YTDTallyPage : ContentPage
	{
		public YTDTallyPage()
		{
            this.BindingContext = new YTDViewModel();
			InitializeComponent ();
		}

        async void AddWaste_Clicked(object sender, EventArgs e) 
        {
            await Navigation.PushAsync(new AddWastePage());
        }

	}
}