using MvvmHelpers;

using Xamarin.Forms;
using OSL.Models;
using Acr.UserDialogs;

namespace OSL
{
    public class ViewModelBase : BaseViewModel
    {
        public IDataStore<Donation> DataStore => DependencyService.Get<IDataStore<Donation>>() ?? new MockDataStore();


        public void ShowFailureDialog(string title)
        {
            var alertConfig = new AlertConfig
            {
                Title = title,
                Message = "Please try again later."
            };
            UserDialogs.Instance.Alert(alertConfig);
        }
    }
}
