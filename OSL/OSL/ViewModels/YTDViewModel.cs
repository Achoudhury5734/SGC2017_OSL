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
        public Command LoadAmountsCommand { get; set; }
        public Command AddWasteCommand { get; set; }
        public PlotModel Model { get; set; }

        private int yearWasted;
        private int yearDonated;
        private int listed;
        private int pending;

        public string YearWasted { get { return yearWasted + " lbs"; } }
        public string YearDonated { get { return yearDonated + " lbs"; } }
        public string Listed { get { return listed + " lbs"; } }
        public string Pending { get { return pending + " lbs"; } }

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
                OnPropertyChanged("Listed");

                pending = userStats[(int)DonationStatus.PendingPickup];
                OnPropertyChanged("Pending");

                yearDonated = userStats[(int)DonationStatus.Completed];
                OnPropertyChanged("YearDonated");

                yearWasted = userStats[(int)DonationStatus.Wasted];
                OnPropertyChanged("YearWasted");

                Model = GeneratePlotModel();
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

        private PlotModel GeneratePlotModel()
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

            ps.Slices.Add(new PieSlice("", listed) { Fill = OxyColor.Parse("#eb851d") });
            ps.Slices.Add(new PieSlice("", pending) { Fill = OxyColor.Parse("#f9e255") });
            ps.Slices.Add(new PieSlice("", yearDonated) {Fill = OxyColor.Parse("#42b858") });
            ps.Slices.Add(new PieSlice("", yearWasted) {Fill = OxyColor.Parse("#e33033") });
            ps.FontSize = 15.5;

            ps.TickHorizontalLength = 0.00;
            ps.TickRadialLength = 0.00;
            ps.OutsideLabelFormat = "";

            model.Series.Add(ps);

            return model;
        }

        async Task ExecuteAddWasteCommand()
        {
            var promptConfig = new PromptConfig();
            promptConfig.Title = "Add to Wasted Food Count";
            promptConfig.OkText = "Add";
            promptConfig.Placeholder = "Quantity (lbs)";
            var response = await UserDialogs.Instance.PromptAsync(promptConfig);

            int? amount = ProcessInput(response);
            if (amount.HasValue) {
                var result = await wasteRep.CreateWaste(amount.Value);
                if (!result)
                    UserDialogs.Instance.Alert("Something went wrong.\nPlease try again later.");
                else
                {
                    yearWasted += amount.Value;
                    OnPropertyChanged("YearWasted");
                    Model = GeneratePlotModel();
                    OnPropertyChanged("Model");
                }
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
