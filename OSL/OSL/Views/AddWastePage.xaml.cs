using System;
using System.Collections.Generic;
using OSL.Models;
using OSL.Services;
using Xamarin.Forms;

namespace OSL.Views
{
    public partial class AddWastePage : ContentPage
    {
        public AddWastePage()
        {
            InitializeComponent();
            addWaste.Clicked += AddWaste_Clicked;
        }


        async void AddWaste_Clicked(object sender, EventArgs e)
        {
            WasteRepository wasteRep = new WasteRepository();
            Donation waste = new Donation();
            waste.Amount = int.Parse(newWaste.Text);
            waste.Status = DonationStatus.Wasted;
            var res = await wasteRep.CreateWaste(waste);

            if (res == null)
                await DisplayAlert("Oops", "Something went wrong. Please try again later", "Ok");
        }
    }
}
