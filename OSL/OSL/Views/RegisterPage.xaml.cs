using System;
using OSL.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OSL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public Models.User User;
        RegistrationViewModel viewModel;

        public RegisterPage()
        {
            User = new Models.User();
            viewModel = new RegistrationViewModel();
            BindingContext = viewModel;
            InitializeComponent();

            MessagingCenter.Subscribe<RegistrationViewModel, string>(this, "BadRegistrationAlert", BadRegistration);
            MessagingCenter.Subscribe<RegistrationViewModel>(this, "GoodRegistrationAlert", GoodRegistration);
        }

        private void BadRegistration(RegistrationViewModel obj, string message)
        {
            DisplayAlert("Could not register", message, "Ok");
        }

        private void GoodRegistration(RegistrationViewModel obj) 
        {
            DisplayAlert("Thanks for signing up!",
                         "We'll let you know when you're verified. Feel free to look around!", "Ok");
            Application.Current.MainPage = new RootPage();
        }
    }
}
