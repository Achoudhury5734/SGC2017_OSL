using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Acr.UserDialogs;
using OSL.Models;
using OSL.Services;
using OxyPlot;
using OxyPlot.Series;
using Xamarin.Forms;

namespace OSL.ViewModels
{
    public class YTDViewModel : ViewModelBase
    {
        public string YearWasted { get; set; }
        public string YearDonated { get; set; }
        public string Listed { get; set; }
        public string Pending { get; set; }
        public Command LoadAmountsCommand { get; set; }
        public Command AddWasteCommand { get; set; }
        public PlotModel Model { get; set; }

        private double yearWasted;
        private double yearDonated;
        private double listed;
        private double pending;

        private readonly WasteRepository wasteRep;

        public YTDViewModel()
        {
            wasteRep = new WasteRepository();
            LoadAmountsCommand = new Command(async () => await ExecuteLoadAmounts());
            AddWasteCommand = new Command(async () => await ExecuteAddWasteCommand());
            Model = new PlotModel();
            notBusy = false;
        }

        private bool notBusy;
        public bool NotBusy
        {
            get{ return notBusy; }
            set { notBusy = value; }
        }

        async Task ExecuteLoadAmounts() 
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var userStats = await wasteRep.GetDonorStats();

                listed = userStats[(int)DonationStatus.Listed];
                Listed = listed + " lbs";
                OnPropertyChanged("Listed");

                pending = userStats[(int)DonationStatus.PendingPickup];
                Pending = pending + " lbs";
                OnPropertyChanged("Pending");

                yearDonated = userStats[(int)DonationStatus.Completed];
                YearDonated = yearDonated + " lbs";
                OnPropertyChanged("YearDonated");

                yearWasted = userStats[(int)DonationStatus.Wasted];
                YearWasted = yearWasted + " lbs";
                OnPropertyChanged("YearWasted");

                Model = generatePlotModel();
                OnPropertyChanged("Model");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
                NotBusy = true;
                OnPropertyChanged("NotBusy");
            }
        }

        private PlotModel generatePlotModel()
        {
            var model = new PlotModel
            {
                Title = DateTime.Now.Year + "",
                PlotAreaBorderColor = OxyColors.Transparent
            };
            var ps = new PieSeries
            {
                StrokeThickness = 0.0,
                InsideLabelPosition = 0.5,
                AngleSpan = 360,
                StartAngle = 0,
                InnerDiameter = 0.4
            };

            ps.Slices.Add(new PieSlice("", listed) { Fill = OxyColors.DarkOrange });
            ps.Slices.Add(new PieSlice("", pending) { Fill = OxyColors.Yellow });
            ps.Slices.Add(new PieSlice("", yearDonated) {Fill = OxyColors.LimeGreen });
            ps.Slices.Add(new PieSlice("", yearWasted) {Fill = OxyColors.Red });
            ps.FontSize = 15.5;

            ps.TickHorizontalLength = 0.0;
            ps.TickRadialLength = 0.0;
            ps.OutsideLabelFormat = "";

            model.Series.Add(ps);

            return model;
        }

        async Task ExecuteAddWasteCommand()
        {
            var promptConfig = new PromptConfig();
            promptConfig.Message = "Add to Wasted Food Count";
            promptConfig.OkText = "Add";
            promptConfig.Placeholder = "Quantity (lbs)";
            var response = await UserDialogs.Instance.PromptAsync(promptConfig);

            int? amount = ProcessInput(response);
            if (amount.HasValue) {
                var result = await wasteRep.CreateWaste(amount.Value);
                if (!result)
                    UserDialogs.Instance.Alert("Something went wrong.\nPlease try again later.");
                else
                    await ExecuteLoadAmounts();
            }
        }

        int? ProcessInput(PromptResult response)
        {
            if (response.Ok && response != null)
            {
                var digits = Regex.Replace(response.Value, "[^0-9.]", "");
                if (!String.IsNullOrWhiteSpace(digits))
                {
                    // Round to nearest int if user entered double
                    var amount = Convert.ToInt32(double.Parse(digits));
                    if (amount > 0)
                        return amount;
                }
            }
            return null;
        }
    }
}
