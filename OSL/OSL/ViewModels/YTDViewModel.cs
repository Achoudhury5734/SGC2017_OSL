using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
        public PlotModel Model { get; set; }

        private double yearWasted;
        private double yearDonated;
        private double listed;
        private double pending;

        private readonly WasteRepository wasteRep;

        public YTDViewModel()
        {
            wasteRep = new WasteRepository();
            LoadAmountsCommand = new Command(async () => await ExecuteLoadAmountsCommand());
            Model = new PlotModel();
            notBusy = false;
        }

        private bool notBusy;
        public bool NotBusy
        {
            get{ return notBusy; }
            set { notBusy = value; }
        }

        async Task ExecuteLoadAmountsCommand() 
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var userDonations = await wasteRep.GetYearDonorItems();

                yearWasted = queryDonorItems(DonationStatus.Wasted, userDonations);
                YearWasted = yearWasted + " lbs"; 
                OnPropertyChanged("YearWasted");

                yearDonated = queryDonorItems(DonationStatus.Completed, userDonations);
                YearDonated = yearDonated + " lbs";
                OnPropertyChanged("YearDonated");

                listed = queryDonorItems(DonationStatus.Listed, userDonations);
                Listed = listed + " lbs";
                OnPropertyChanged("Listed");

                pending = queryDonorItems(DonationStatus.PendingPickup, userDonations);
                Pending = pending + " lbs";
                OnPropertyChanged("Pending");

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

        private int queryDonorItems(DonationStatus status, IEnumerable<Donation> items) 
        {
            var query = from item in items
                             where item.Status == status
                             select item.Amount;
            return query.Sum();
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
                StrokeThickness = 2.0,
                InsideLabelPosition = 0.5,
                AngleSpan = 360,
                StartAngle = 0,
                InnerDiameter = 0.4
            };

            // Over the year pending and listed should be way smaller and labels won't fit
            ps.Slices.Add(new PieSlice("", listed) { Fill = OxyColors.DarkOrange });
            ps.Slices.Add(new PieSlice("", pending) { Fill = OxyColors.Yellow });
            ps.Slices.Add(new PieSlice("Donated", yearDonated) {Fill = OxyColors.LimeGreen });
            ps.Slices.Add(new PieSlice("Wasted", yearWasted) {Fill = OxyColors.Red });
            ps.FontSize = 15.5;

            ps.TickHorizontalLength = 0.0;
            ps.TickRadialLength = 0.0;
            ps.OutsideLabelFormat = "";

            model.Series.Add(ps);

            return model;
        }
    }
}
