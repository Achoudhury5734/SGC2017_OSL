using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using OSL.Models;
using OSL.Services;
using Xamarin.Forms;

namespace OSL.ViewModels
{
    public class AcceptedDetailViewModel: RecipientDetailBase
    {
        public Command OptionsCommand { get; }
        public Page Page { get; set; }
        public bool HasImage { get; set; }

        DonationRepository donationRep;

        public AcceptedDetailViewModel(Donation item = null)
        {
            Title = item?.Title;
            Item = item;

            donationRep = new DonationRepository();
            OptionsCommand = new Command(ExecuteOptionsCommand);

            if (String.IsNullOrWhiteSpace(item.PictureUrl) || String.Equals(item.PictureUrl, "Empty"))
            {
                item.PictureUrl = null;
                HasImage = false;
            }
            else
            {
                HasImage = true;
            }
        }

        void ExecuteOptionsCommand()
        {
            var actionConfig = new ActionSheetConfig();
            actionConfig.Add("Contact Donor", ExecuteOpenDialer);
            actionConfig.Add("View in Maps", async () => await ExecuteOpenMaps());
            if (Item.Status != DonationStatus.Completed)
                actionConfig.Add("Cancel Pickup", (async () => await ExecuteCancelCommand()));
            actionConfig.SetCancel("Close");

            UserDialogs.Instance.ActionSheet(actionConfig);
        }

        private async Task ExecuteCancelCommand()
        {
            var res = await donationRep.CancelDonationAsync(Item.Id);
            if (!res)
            {
                ShowFailureDialog("Unable to Cancel Pickup");
            }
            else
            {
                MessagingCenter.Send(this, "PickupCancelled", Item);
                await Page.Navigation.PopAsync();
            }
        }

        private bool CanCancel() => Item.Status == DonationStatus.PendingPickup;
    }
}
