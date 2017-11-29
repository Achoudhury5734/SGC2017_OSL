using System;

namespace OSL
{
    public class PickupItemDetailViewModel : ViewModelBase
    {
        public PickupItem Item { get; set; }
        public PickupItemDetailViewModel(PickupItem item = null)
        {
            Title = item?.Title;
            Item = item;
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
    }
}
