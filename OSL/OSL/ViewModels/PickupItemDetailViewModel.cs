using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Plugin.ExternalMaps;
using Plugin.Messaging;
using Xamarin.Forms;

namespace OSL
{
    public class PickupItemDetailViewModel : ViewModelBase
    {
        public PickupItem Item { get; set; }
        public Command OptionsCommand { get; }
        public PickupItemDetailViewModel(PickupItem item = null)
        {
            Title = item?.Title;
            Item = item;
            OptionsCommand = new Command(() => ExecuteOptions());
        }

        public string Address
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(Item.Donor.OrganizationAddressLine2))
                {
                    return string.Format("{0}\n{1}\n{2}, {3} {4}",
                                         Item.Donor.OrganizationAddressLine1,
                                         Item.Donor.OrganizationAddressLine2,
                                         Item.Donor.OrganizationCity,
                                         Item.Donor.OrganizationState,
                                         Item.Donor.OrganizationPostalCode
                                        );
                }
                else
                {
                    return string.Format("{0}\n{1}, {2} {3}",
                                         Item.Donor.OrganizationAddressLine1,
                                         Item.Donor.OrganizationCity,
                                         Item.Donor.OrganizationState,
                                         Item.Donor.OrganizationPostalCode
                                        );
                }
            }
        }

        private async Task ExecuteOpenMaps() {
            var success = await CrossExternalMaps.Current.NavigateTo(Item.Donor.OrganizationName, 
                                                                     Item.Donor.OrganizationAddressLine1, 
                                                                     Item.Donor.OrganizationCity, 
                                                                     Item.Donor.OrganizationState,
                                                                     Item.Donor.OrganizationPostalCode,
                                                                     Item.Donor.OrganizationCountry, 
                                                                     Item.Donor.OrganizationCountry);
            if (!success) {
                UserDialogs.Instance.Alert("Unable to Open Map");
            }
        }

        private void ExecuteOpenDialer() {
            var phoneDialer = CrossMessaging.Current.PhoneDialer;
            if (phoneDialer.CanMakePhoneCall)
                phoneDialer.MakePhoneCall(Item.Donor.PhoneNumber);
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
