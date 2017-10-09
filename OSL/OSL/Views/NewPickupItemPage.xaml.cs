using System;

using Xamarin.Forms;

namespace OSL
{
	public partial class NewPickupItemPage : ContentPage
	{
		public PickupItem Item { get; set; }

		public NewPickupItemPage()
		{
			InitializeComponent();

			Item = new PickupItem
			{
				Title = "Item name"
			};

			BindingContext = this;
		}

		async void Save_Clicked(object sender, EventArgs e)
		{
			MessagingCenter.Send(this, "AddItem", Item);
			await Navigation.PopToRootAsync();
		}
	}
}
