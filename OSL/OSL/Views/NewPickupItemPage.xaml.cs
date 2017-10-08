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
				Text = "Item name",
				Description = "This is an item description."
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
