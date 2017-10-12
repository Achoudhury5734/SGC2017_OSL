using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
            waste.Amount = int.Parse(Regex.Replace(newWaste.Text, "[^0-9.]", ""));
            waste.Status = DonationStatus.Wasted;
            await DisplayAlert("Adding waste", waste.Amount.ToString(), "Ok");
            /*var res = await wasteRep.CreateWaste(waste);

            if (res == null)
                await DisplayAlert("Oops", "Something went wrong. Please try again later", "Ok");
            else*/
                await Navigation.PopAsync();
        }
    }
}
