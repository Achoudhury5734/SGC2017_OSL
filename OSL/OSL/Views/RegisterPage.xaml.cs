using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace OSL.Views
{
    public partial class RegisterPage : ContentPage
    {
        public Models.User User;

        public RegisterPage()
        {
            InitializeComponent();
            registerButton.Clicked += handle_Registration;
        }

        private void handle_Registration(object sender, EventArgs e)
        {
            
        }
    }
}
