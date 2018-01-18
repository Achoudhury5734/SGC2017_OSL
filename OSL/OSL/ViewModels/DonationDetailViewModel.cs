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

        public Donation Item { get; set; }

        public Command OpenDialerCommand { get; }
        public Command CompleteCommand { get; }
        public Command WasteCommand { get; }
        public Command EditCommand { get; }
        public Command RemoveCommand { get; }

        public DonationDetailViewModel(Donation item)
        {
            Title = item?.Title;
            Item = item;

            donationRepository = new DonationRepository();

            CompleteCommand = new Command(async () => await CompleteDonationAsync(item.Id), () => CanCompleteDonation);
            WasteCommand = new Command(async () => await WasteDonationAsync(item.Id), () => CanWasteDonation);
            EditCommand = new Command(async () => await EditDonationAsync(item), () => CanEditDonation);
            RemoveCommand = new Command(async () => await RemoveRecipientAsync(item.Id), () => CanRemoveRecipient);
            OpenDialerCommand = new Command(ExecuteOpenDialer);
        }

        // For XAML formatting
        public bool HasRecipient { get { return Item.Recipient != null; } }
        public bool HasNoRecipient { get { return Item.Recipient == null; }}
        public bool ShowRelistButton { get { return Item.Status == DonationStatus.Wasted; } }
        public bool ShowEditButton 
        { 
            get 
            {
                return Item.Status == DonationStatus.Listed || Item.Status == DonationStatus.PendingPickup; 
            } 
        }

        public bool HasImage
        {
            get
            {
                return !String.IsNullOrWhiteSpace(Item.PictureUrl) && !String.Equals(Item.PictureUrl, "Empty");
            }
        }

        private void ExecuteOpenDialer()
        {
            var phoneDialer = CrossMessaging.Current.PhoneDialer;
            if (phoneDialer.CanMakePhoneCall)
                phoneDialer.MakePhoneCall(Item.Recipient.Phone_Number);
            else
                UserDialogs.Instance.Alert("Unable to Make Calls");
        }

        private async Task CompleteDonationAsync(int donationId)
        {
            var res = await donationRepository.CompleteDonationAsync(donationId);
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

        private async Task WasteDonationAsync(int donationId)
        {
            var res = await donationRepository.WasteDonationAsync(donationId);
            if (res) {
                MessagingCenter.Send(this, "StatusChanged", Item);
                await Page.Navigation.PopAsync();
            } else {
                ShowFailureDialog("Unable to Waste Donation");
            }
        }

        private async Task RemoveRecipientAsync(int donationId)
        {
            var confirmed = await CheckRemoveRecipient();
            if (confirmed)
            {
                var res = await donationRepository.RemoveRecipientAsync(donationId);
                if (res)
                {
                    MessagingCenter.Send(this, "StatusChanged", Item);
                    //App.Current.MainPage = new RootPage() { Detail = new NavigationPage(new DonationTabPage()) };
                    await Page.Navigation.PopAsync();
                }
                else
                {
                    ShowFailureDialog("Unable to Remove Recipient");
                }
            }
        }

        private async Task<bool> CheckRemoveRecipient() {
            var res = true;
            if (Item.Recipient != null) {
                var confirmConfig = new ConfirmConfig();
                confirmConfig.OkText = "Yes";
                confirmConfig.Message = "Are you sure you want to remove them?";
                confirmConfig.Title = $"This means {Item.Recipient.Person_Name} won't pick up your donation.";
                res = await UserDialogs.Instance.ConfirmAsync(confirmConfig);
            }
            return res;
        }

        private async Task EditDonationAsync(Donation donation)
        {
            await Page.Navigation.PushAsync(new DonationPage(donation));
        }

        private Page page;
        public Page Page
        {
            get { return page; }
            set { SetProperty(ref page, value); }
        }

        public bool CanRemoveRecipient { get { return Item.Status == DonationStatus.PendingPickup; }}
        public bool CanCompleteDonation { get { return Item.Status == DonationStatus.PendingPickup; } }
        public bool CanEditDonation { get { return Item.Status != DonationStatus.Completed; } }
        public bool CanWasteDonation
        {
            get
            {
                return Item.Status != DonationStatus.Wasted && Item.Status != DonationStatus.Completed;
            }
        }
    }
}
