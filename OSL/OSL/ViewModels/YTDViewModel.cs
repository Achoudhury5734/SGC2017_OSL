using System;
using System.Diagnostics;
using System.Threading.Tasks;
using OSL.Services;
using Xamarin.Forms;

namespace OSL.ViewModels
{
    public class YTDViewModel : ViewModelBase
    {
        public string YearWasted { get; set; }
        public string YearDonated { get; set; }
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
                YearWasted = wasteRep.GetDonorWaste(userDonations) + " lbs";
                OnPropertyChanged("YearWasted");
                YearDonated = wasteRep.GetDonorComplete(userDonations) + " lbs";
                OnPropertyChanged("YearDonated");
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
    }
}
