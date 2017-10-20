using System;
using System.Collections.Generic;
using System.Text;
using OSL.Models;

namespace OSL.ViewModels
{
    public class DonationDetailViewModel : ViewModelBase
    {
        public Donation Item { get; set; }

        public DonationDetailViewModel(Donation item)
        {
            Title = item?.Title;
            Item = item;
        }
    }
}
