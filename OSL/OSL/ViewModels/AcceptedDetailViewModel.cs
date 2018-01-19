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
        public Command OpenDialerCommand { get; }
        public Command OpenMapsCommand { get; }
        public Command CancelCommand { get; }
        public Command CompleteCommand { get; }
        public Page Page { get; set; }
        public bool HasImage { get; set; }

        DonationRepository donationRep;

        public AcceptedDetailViewModel(Donation item = null)
        {
            Title = item?.Title;
            Item = item;

            donationRep = new DonationRepository();
            OpenMapsCommand = new Command(async () => await ExecuteOpenMaps());
            OpenDialerCommand = new Command(ExecuteOpenDialer);
            CompleteCommand = new Command(async () => await CompleteDonationAsync());
            CancelCommand = new Command(async () => await ExecuteCancelCommand());

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

        private async Task ExecuteCancelCommand()
        {
            var res = await donationRep.CancelDonationAsync(Item.Id);
            if (!res)
            {
                ShowFailureDialog("Unable to Cancel Pickup");
            }
            else
            {
                MessagingCenter.Send(this, "StatusChanged", Item);
                await Page.Navigation.PopAsync();
            }
        }

        private async Task CompleteDonationAsync()
        {
            var res = await donationRep.CompleteDonationAsync(Item.Id);
            if (res)
            {
                MessagingCenter.Send(this, "StatusChanged", Item);
                await Page.Navigation.PopAsync();
            }
            else
            {
                ShowFailureDialog("Unable to Complete Donation");
            }
        }

        public bool CanChangeStatus { get { return Item.Status == DonationStatus.PendingPickup; } }
    }
}
