using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OSL.Services;
using Xamarin.Forms;

namespace OSL.Views
{
    public partial class RegisterPage : ContentPage
    {
        public Models.User User;

        public RegisterPage()
        {
            User = new Models.User();
            InitializeComponent();
            registerButton.Clicked += Handle_Registration;
        }

        private async void Handle_Registration(object sender, EventArgs e)
        {
            User.Organization_Country = "USA";
            User.Organization_Name = orgName.Text;
            User.Organization_City = orgCity.Text;
            User.Organization_Address_Line1 = orgAddress1.Text;
            User.Organization_Address_Line2 = orgAddress2.Text;
            User.Organization_PostalCode = orgZip.Text;
            User.Person_Name = orgName.Text;
            User.Phone_Number = phoneNumber.Text;
            User.Person_Name = personName.Text;
            User.Organization_State = orgState.Text;

            foreach (var pi in User.GetType().GetProperties())
            {
                if (String.IsNullOrEmpty((string)pi.GetValue(User)))  
                {
                    if (!pi.Name.Equals("Organization_Address_Line2")) 
                    {
                        await DisplayAlert("Unable to register",
                                           "Please fill out all fields", "Ok");
                        return;
                    }
                }
            }
            await DisplayAlert("Thanks for signing up",
                         "We'll let you know when you're verified. Feel free to look around!", "Ok");
            /*var userRep = new UserRepository();
            var res = await userRep.Create(User);
            if (res == null)
                await DisplayAlert("Oops", "Something went wrong", "Ok");
            else
                Application.Current.MainPage = new RootPage();*/
		}

        private void BadFormAlert(string field)
        {
            DisplayAlert("Unable to register", "Please give a " + field, "Ok");
        }
    }
}
