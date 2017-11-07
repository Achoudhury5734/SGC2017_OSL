using System;
using System.Threading.Tasks;
using OSL.Models;
using OSL.Services;
using Xamarin.Forms;

namespace OSL.ViewModels
{
    public class AcceptedDetailViewModel: ViewModelBase
    {
        public Donation Item { get; set; }
        public Command CancelCommand { get; set; }
        public Page Page { get; set; }

        DonationRepository donationRep;

        public AcceptedDetailViewModel(Donation item = null)
        {
            Title = item?.Title;
            Item = item;

            donationRep = new DonationRepository();

            CancelCommand = new Command(async () => await ExecuteCancelCommand());
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
            await donationRep.CancelDonationAsync(Item.Id); 
            await Page.Navigation.PopAsync();
        }
    }
}
