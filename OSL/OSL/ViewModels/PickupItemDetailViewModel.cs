using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using OSL.Models;
using Plugin.ExternalMaps;
using Plugin.Messaging;
using Xamarin.Forms;

namespace OSL
{
    public class PickupItemDetailViewModel : ViewModelBase
    {
        public Donation Item { get; set; }
        public Command OptionsCommand { get; }
        public PickupItemDetailViewModel(Donation item = null)
        {
            Title = item?.Title;
            Item = item;
            OptionsCommand = new Command(() => ExecuteOptions());
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

        private async Task ExecuteOpenMaps() {
            var success = await CrossExternalMaps.Current.NavigateTo(Item.Donor.Organization_Name, 
                                                                     Item.Donor.Organization_Address_Line1, 
                                                                     Item.Donor.Organization_City, 
                                                                     Item.Donor.Organization_State,
                                                                     Item.Donor.Organization_PostalCode,
                                                                     Item.Donor.Organization_Country, 
                                                                     Item.Donor.Organization_Country);
            if (!success) {
                UserDialogs.Instance.Alert("Unable to Open Map");
            }
        }

        private void ExecuteOpenDialer() {
            var phoneDialer = CrossMessaging.Current.PhoneDialer;
            if (phoneDialer.CanMakePhoneCall)
                phoneDialer.MakePhoneCall(Item.Donor.Phone_Number);
            else
                UserDialogs.Instance.Alert("Unable to Make Calls");
        }

        private void ExecuteOptions()
        {
            var actionConfig = new ActionSheetConfig();
            actionConfig.Add("Contact Donor", () => ExecuteOpenDialer());
            actionConfig.Add("View in Maps", async () => await ExecuteOpenMaps());
            actionConfig.SetCancel();

            UserDialogs.Instance.ActionSheet(actionConfig);
        }
    }
}
