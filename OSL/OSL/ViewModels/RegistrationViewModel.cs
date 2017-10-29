using System;
using System.Threading.Tasks;
using System.Windows.Input;
using OSL.Models;
using OSL.Services;
using Xamarin.Forms;
using System.Reflection;

namespace OSL.ViewModels
{
    public class RegistrationViewModel : ViewModelBase
    {
        public User User { get; set; }
        public ICommand RegisterUserCommand { get; }

        private readonly UserRepository userRep;

        public RegistrationViewModel()
        {
            User = new User();
            userRep = new UserRepository();
            RegisterUserCommand = new Command(async () => await RegisterUserAsync());
        }

        private async Task RegisterUserAsync()
        {
            User.Organization_Country = "USA";
            var full = CheckFullRegistration();
            if (full)
            {
                var res = await userRep.Create(User);
                if (res == null)
                {
                    MessagingCenter.Send(this, "BadRegistrationAlert", "Something went wrong, please try again later");
                }
                else
                {
                    App.User = res;
                    MessagingCenter.Send(this, "GoodRegistrationAlert");
                }
            }
            else 
            {
                MessagingCenter.Send(this, "BadRegistrationAlert", "Please fill out all required fields");
            }
        }

        private bool CheckFullRegistration()
        {
            foreach (var pi in User.GetType().GetProperties())
            {
                if (String.IsNullOrEmpty((string)pi.GetValue(User)))
                {
                    if (!pi.Name.Equals("Organization_Address_Line2"))
                        return false;
                }
            }
            return true;
        }
    }
}
