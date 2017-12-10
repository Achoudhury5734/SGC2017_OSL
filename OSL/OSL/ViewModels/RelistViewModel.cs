using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using OSL.Models;
using OSL.Services;
using OSL.Views;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace OSL.ViewModels
{
    public class RelistViewModel : ViewModelBase
    {
        private readonly DonationRepository donationRep;

        public string PageTitle { get; set; }
        public string EnterText { get; set; }
        public Command EnterCommand { get; }
        public Command TakePictureCommand { get; }
        public Page Page;

        private Donation donation;

        public RelistViewModel(Donation donation)
        {
            donationRep = new DonationRepository();
            PageTitle = "Edit Item";
            EnterText = "Relist";
            EnterCommand = new Command(async () => await ExecuteRelistCommand(donation.Id), () => !IsBusy);
            TakePictureCommand = new Command(async () => await ExecuteTakePicture());

            ImageSource = donation.PictureUrl;
            DonationTitle = donation.Title;
            Quantity = donation.Amount;
            DonationType = donation.Type.ToString();

            var expiration = getExpiration(donation.Expiration.Value);
            ExpirationDate = expiration.Date;
            ExpirationTime = new TimeSpan(expiration.Hour, expiration.Minute, expiration.Second);

            this.donation = donation;
        }

        public ImageSource ImageSource { get; set; }
        public string DonationTitle { get; set; }
        public int Quantity { get; set; }
        public string DonationType { get; set; }
        public DateTime ExpirationDate { get; set; }
        public TimeSpan ExpirationTime { get; set; }

        private DateTime getExpiration(DateTime oldExpiration) {
            DateTime expiration;
            if (oldExpiration < DateTime.Now)
            {
                // If expired, set new expiration to two hours from now
                expiration = DateTime.Now.AddHours(2);
            }
            else
            {
                expiration = oldExpiration;
            }
            return expiration;
        }

        private async Task ExecuteRelistCommand(int id)
        {
            DonationCapture capture = new DonationCapture()
            {
                Title = DonationTitle,
                Type = DonationType,
                Amount = Quantity,
                Expiration = ExpirationDate.Add(ExpirationTime)
            };
            var okToProceed = await CheckRemoveRecipient();
            if (okToProceed)
            {
                IsBusy = true;
                EnterCommand.ChangeCanExecute();
                var res = await donationRep.RelistDonationAsync(capture, donation.Id, mediaFile);
                IsBusy = false;
                EnterCommand.ChangeCanExecute();

                if (!res)
                    ShowFailureDialog("Unable to Relist");
                else
                    App.Current.MainPage = new RootPage() { Detail = new NavigationPage(new DonationTabPage()) };
            }
        }

        private async Task<bool> CheckRemoveRecipient() {
            var res = true;
            if (donation.Recipient != null) {
                var confirmConfig = new ConfirmConfig();
                confirmConfig.CancelText = "Cancel";
                confirmConfig.OkText = "Relist anyway";
                confirmConfig.Message = "This donation has been accepted. If you relist the recipient will be removed.";
                confirmConfig.Title = "Warning";
                res = await UserDialogs.Instance.ConfirmAsync(confirmConfig);
            }
            return res;
        }

        private MediaFile mediaFile;
        private async Task ExecuteTakePicture()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                UserDialogs.Instance.Alert("","No Camera Available");
                return;
            }

            mediaFile = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "image.jpg"
            });

            if (mediaFile == null)
                return;
            
            ImageSource = ImageSource.FromStream(() =>
            {
                var stream = mediaFile.GetStream();
                return stream;
            });
            OnPropertyChanged("ImageSource");
            return;
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
