using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using OSL.Models;
using OSL.ViewModels;
using Xamarin.Forms;

namespace OSL
{
    public class PickupItemDetailViewModel : RecipientDetailBase
    {
        private CloudDataStore dataStore;

        public Command OptionsCommand { get; }
        public Page Page { get; set; }
        public Command AcceptCommand { get; }
        public PickupItemDetailViewModel(Donation item = null)
        {
            Title = item?.Title;
            Item = item;
            OptionsCommand = new Command(ExecuteOptions);
            AcceptCommand = new Command(async () => await ExecuteAcceptCommand());
            dataStore = new CloudDataStore();
        }

        private void ExecuteOptions()
        {
            var actionConfig = new ActionSheetConfig();
            actionConfig.Add("Contact Donor", ExecuteOpenDialer);
            actionConfig.Add("View in Maps", async () => await ExecuteOpenMaps());
            actionConfig.SetCancel();

            UserDialogs.Instance.ActionSheet(actionConfig);
        }

        private async Task ExecuteAcceptCommand() {
            var res = await dataStore.AcceptPickupItemAsync(Item);
            if (res)
            {
                await UserDialogs.Instance.AlertAsync($"Successfully accepted {Item.Title}.", "Item Accepted");
                await Page.Navigation.PopAsync();
                MessagingCenter.Send(this, "ItemAccepted", Item);
            }
            else
            {
                ShowFailureDialog("Unable to Accept Donation");
            }
        }
    }
}
