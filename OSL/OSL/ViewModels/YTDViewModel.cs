using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using OSL.Models;
using OSL.Services;
using Xamarin.Forms;

namespace OSL.ViewModels
{
    public class YTDViewModel : ViewModelBase
    {
        public string YearWasted { get; set; }
        public string YearDonated { get; set; }
        public string Listed { get; set; }
        public int CurrentYear { get; set; }
        public Command LoadAmountsCommand { get; set; }

        private readonly WasteRepository wasteRep;

        public YTDViewModel()
        {
            wasteRep = new WasteRepository();
            CurrentYear = DateTime.Now.Year;
            LoadAmountsCommand = new Command(async () => await ExecuteLoadAmountsCommand());
        }

        async Task ExecuteLoadAmountsCommand() 
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var userDonations = await wasteRep.GetYearDonorItems(13);
                YearWasted = queryDonorItems(DonationStatus.Wasted, userDonations) + " lbs"; 
                OnPropertyChanged("YearWasted");
                YearDonated = queryDonorItems(DonationStatus.Completed, userDonations) + " lbs";
                OnPropertyChanged("YearDonated");
                Listed = queryDonorItems(DonationStatus.Listed, userDonations) + " lbs";
                OnPropertyChanged("Listed");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private int queryDonorItems(DonationStatus status, IEnumerable<Donation> items) 
        {
            var query = from item in items
                             where item.Status == status
                             select item.Amount;
            return query.Sum();
        }
    }
}
