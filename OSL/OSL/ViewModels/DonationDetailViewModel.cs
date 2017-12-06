using System;
using System.Threading.Tasks;
using OSL.Models;
using Xamarin.Forms;
using OSL.Services;
using OSL.Views;
using Acr.UserDialogs;
using Plugin.Messaging;

namespace OSL.ViewModels
{
    public class DonationDetailViewModel : ViewModelBase
    {
        DonationRepository donationRepository;

        public DonationDetailViewModel(Donation item)
        {
            Title = item?.Title;
            Item = item;

            donationRepository = new DonationRepository();

            CompleteCommand = new Command(async () => await CompleteDonationAsync(item.Id), () => CanCompleteDonation(item.Status));
            WasteCommand = new Command(async () => await WasteDonationAsync(item.Id), () => CanWasteDonation(item.Status));
            RelistCommand = new Command(async () => await RelistDonationAsync(item), () => CanRelistDonation(item.Status));
            OpenDialerCommand = new Command(() => ExecuteOpenDialer());
            OptionsCommand = new Command(() => ExecuteOptions());

        }
        public Donation Item { get; set; }
        public bool HasRecipient { get { return Item.Recipient != null; } }
        public bool HasNoRecipient { get { return Item.Recipient == null; }}
        public Command OpenDialerCommand { get; }
        public Command OptionsCommand { get; }

        private void ExecuteOpenDialer()
        {
            var phoneDialer = CrossMessaging.Current.PhoneDialer;
            if (phoneDialer.CanMakePhoneCall)
                phoneDialer.MakePhoneCall(Item.Recipient.Phone_Number);
            else
                UserDialogs.Instance.Alert("Unable to Make Calls");
        }

        public bool HasImage { 
            get 
            {
                return !String.IsNullOrWhiteSpace(Item.PictureUrl) && !String.Equals(Item.PictureUrl, "Empty");
            }
        }

        private async Task CompleteDonationAsync(int donationId)
        {
            await donationRepository.CompleteDonationAsync(donationId);
            await Page.Navigation.PopAsync();
        }

        private async Task WasteDonationAsync(int donationId)
        {
            await donationRepository.WasteDonationAsync(donationId);
            await Page.Navigation.PopAsync();
        }

        private async Task RelistDonationAsync(Donation donation)
        {
            await Page.Navigation.PushAsync(new DonationPage(donation));
        }

        private Page page;
        public Page Page
        {
            get { return page; }
            set { SetProperty(ref page, value); }
        }

        public Command CompleteCommand { get; set; }
        public Command WasteCommand { get; set; }
        public Command RelistCommand { get; set; }

        private bool CanCompleteDonation(DonationStatus status)
        {
            return status == DonationStatus.PendingPickup;
        }

        private bool CanWasteDonation(DonationStatus status)
        {
            return status != DonationStatus.Wasted && status != DonationStatus.Completed; 
        }

        private bool CanRelistDonation(DonationStatus status)
        {
            return status != DonationStatus.Completed;
        }

        void ExecuteOptions()
        {
            var actionConfig = new ActionSheetConfig();
            actionConfig.Title = "Additional Options";
            actionConfig.Add("Contact Recipient", () => ExecuteOpenDialer());
            actionConfig.SetCancel();
            UserDialogs.Instance.ActionSheet(actionConfig);
        }
    }
}
