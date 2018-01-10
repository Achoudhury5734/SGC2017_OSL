using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using OSL.Models;
using Plugin.ExternalMaps;
using Plugin.Messaging;

namespace OSL.ViewModels
{
    public class RecipientDetailBase : ViewModelBase
    {
        public Donation Item { get; set; }

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

        protected async Task ExecuteOpenMaps()
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

        protected void ExecuteOpenDialer()
        {
            var phoneDialer = CrossMessaging.Current.PhoneDialer;
            if (phoneDialer.CanMakePhoneCall)
                phoneDialer.MakePhoneCall(Item.Donor.Phone_Number);
            else
                UserDialogs.Instance.Alert("Unable to Make Calls");
        }
    }
}
