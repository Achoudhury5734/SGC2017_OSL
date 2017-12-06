using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using OSL.Models;
using OSL.Services;
using Plugin.ExternalMaps;
using Plugin.Messaging;
using Xamarin.Forms;

namespace OSL.ViewModels
{
    public class AcceptedDetailViewModel: ViewModelBase
    {
        public Donation Item { get; set; }
        public Command OptionsCommand { get; }
        public Page Page { get; set; }

        DonationRepository donationRep;

        public AcceptedDetailViewModel(Donation item = null)
        {
            Title = item?.Title;
            Item = item;

            donationRep = new DonationRepository();
            OptionsCommand = new Command(() => ExecuteOptionsCommand());
        }

        void ExecuteOptionsCommand()
        {
            var actionConfig = new ActionSheetConfig();
            actionConfig.Add("Contact Donor", () => ExecuteOpenDialer());
            actionConfig.Add("View in Maps", async () => await ExecuteOpenMaps());
            actionConfig.Add("Cancel Pickup", (async () => await ExecuteCancelCommand()));
            actionConfig.SetCancel("Close");

            UserDialogs.Instance.ActionSheet(actionConfig);
        }

        public string Address
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(Item.Donor.Organization_Address_Line2))
                {
                    return string.Format("{0}\n{1}\n{2}, {3} {4}",
                                         Item.Donor.Organization_Address_Line1,
                                         Item.Donor.Organization_Address_Line2,
                                         Item.Donor.Organization_City,
                                         Item.Donor.Organization_State,
                                         Item.Donor.Organization_PostalCode
                                        );
                }
                else
                {
                    return string.Format("{0}\n{1}, {2} {3}",
                                         Item.Donor.Organization_Address_Line1,
                                         Item.Donor.Organization_City,
                                         Item.Donor.Organization_State,
                                         Item.Donor.Organization_PostalCode
                                        );
                }
            }
        }

        private async Task ExecuteCancelCommand()
        {
            var res = await donationRep.CancelDonationAsync(Item.Id);
            if (!res)
            {
                var alertConfig = new AlertConfig();
                alertConfig.Title = "Unable to Cancel Donation";
                alertConfig.Message = "Please try again later.";
                await UserDialogs.Instance.AlertAsync(alertConfig);
            }
            else
            {
                await Page.Navigation.PopAsync();
            }
        }

        private bool CanCancel() => Item.Status == DonationStatus.PendingPickup;

        private async Task ExecuteOpenMaps()
        {
            var success = await CrossExternalMaps.Current.NavigateTo(Item.Donor.Organization_Name,
                                                                     Item.Donor.Organization_Address_Line1,
                                                                     Item.Donor.Organization_City,
                                                                     Item.Donor.Organization_State,
                                                                     Item.Donor.Organization_PostalCode,
                                                                     Item.Donor.Organization_Country,
                                                                     Item.Donor.Organization_Country);
            if (!success)
            {
                UserDialogs.Instance.Alert("Unable to Open Map");
            }
        }

        private void ExecuteOpenDialer()
        {
            var phoneDialer = CrossMessaging.Current.PhoneDialer;
            if (phoneDialer.CanMakePhoneCall)
                phoneDialer.MakePhoneCall(Item.Donor.Phone_Number);
            else
                UserDialogs.Instance.Alert("Unable to Make Calls");
        }
    }
}
