using System;
using System.Threading.Tasks;
using OSL.Services;

namespace OSL.ViewModels
{
    public class YTDViewModel : ViewModelBase
    {
        private string YearWasted { get; set; }
        private string YearDonated { get; set; }

        private readonly WasteRepository wasteRep;

        public YTDViewModel()
        {
            wasteRep = new WasteRepository();
        }

        private async Task<string> GetWasteAmount()
        {
            int wasteAmount = await wasteRep.GetDonorWaste(17);
            return wasteAmount + " lbs";
        }

        private async Task<string> GetDonateAmount()
        {
            int donateAmount = await wasteRep.GetDonorComplete(17);
            return donateAmount + " lbs";
        }
    }
}
