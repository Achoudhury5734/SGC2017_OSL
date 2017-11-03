using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Acr.UserDialogs;
using OSL.Models;
using OSL.Services;
using Xamarin.Forms;

namespace OSL.ViewModels
{
    public class RelistViewModel : ViewModelBase
    {
        private readonly DonationRepository donationRep;

        public string PageTitle { get; set; }
        public string EnterText { get; set; }
        public Command EnterCommand { get; }
        public Command LoadDonationCommand { get; }
        public Command TakePictureCommand { get; }
        public Page Page;

        private Donation donation;

        public RelistViewModel(int id)
        {
            donationRep = new DonationRepository();
            PageTitle = "Relist Item";
            EnterText = "Relist";
            EnterCommand = new Command(async () => await ExecuteRelistCommand(id));
            LoadDonationCommand = new Command(async () => await ExecuteLoadDonations(id));
            TakePictureCommand = new Command(() => ExecuteTakePicture(), () =>false);
        }

        private async Task ExecuteRelistCommand(int id)
        {
            // Doesn't currently support updating picture
            donation.Title = DonationTitle;
            donation.Type = (DonationType)Enum.Parse(typeof(DonationType), DonationType);
            donation.Amount = Quantity;
            donation.Expiration = ExpirationDate.Add(ExpirationTime);

            var res = await donationRep.RelistDonationAsync(donation);
            if (!res)
                ShowFailureDialog("Unable to Relist");
            else
                await Page.Navigation.PopToRootAsync();
                
        }

        private void ExecuteTakePicture()
        {
            return;
        }

        public ImageSource ImageSource { get; set; }
        public string DonationTitle { get; set; }
        public int Quantity { get; set; }
        public string DonationType { get; set; }
        public DateTime ExpirationDate { get; set; }
        public TimeSpan ExpirationTime { get; set; }

        private async Task ExecuteLoadDonations(int id)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                donation = await donationRep.GetDonationAsync(id);
                ImageSource = donation.PictureUrl;
                OnPropertyChanged("ImageSource");
                DonationTitle = donation.Title;
                OnPropertyChanged("DonationTitle");
                Quantity = donation.Amount;
                OnPropertyChanged("Quantity");
                DonationType = donation.Type.ToString();
                OnPropertyChanged("DonationType");
                if (donation.Expiration < DateTime.Now)
                {
                    // If expired, set new expiration to two hours from now
                    var expiration = DateTime.Now.AddHours(2);
                    ExpirationDate = expiration.Date;
                    ExpirationTime = new TimeSpan(expiration.Hour, expiration.Minute, expiration.Second);
                }
                else
                {
                    ExpirationDate = donation.Expiration.Value.Date;
                    ExpirationTime = donation.Expiration.Value.TimeOfDay;
                }
                OnPropertyChanged("ExpirationDate");
                OnPropertyChanged("ExpirationTime");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                ShowFailureDialog("Unable to Load Donation");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ShowFailureDialog(string message)
        {
            var alertConfig = new AlertConfig();
            alertConfig.Title = message;
            alertConfig.Message = "Please try again later.";
            UserDialogs.Instance.Alert(alertConfig);
        }
    }
}
