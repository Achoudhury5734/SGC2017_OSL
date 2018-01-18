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
        public Command EnterCommand { get; }
        public Command TakePictureCommand { get; }
        public Page Page;

        private Donation donation;

        public RelistViewModel(Donation donation)
        {
            donationRep = new DonationRepository();
            PageTitle = "Edit Item";
            EnterCommand = new Command(async () => await ExecuteEditCommand(), () => !IsBusy);
            TakePictureCommand = new Command(async () => await ExecuteTakePicture());

            ImageSource = donation.PictureUrl;
            DonationTitle = donation.Title;
            Quantity = donation.Amount;
            DonationType = donation.Type.ToString();

            var expiration = GetExpiration(donation.Expiration.Value);
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

        private DateTime GetExpiration(DateTime oldExpiration) {
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

        private async Task ExecuteEditCommand()
        {
            DonationCapture capture = new DonationCapture()
            {
                Title = DonationTitle,
                Type = DonationType,
                Amount = Quantity,
                Expiration = ExpirationDate.Add(ExpirationTime)
            };
            bool relisting = donation.Status == DonationStatus.Wasted;
            IsBusy = true;
            EnterCommand.ChangeCanExecute();
            var res = await donationRep.EditDonationAsync(capture, donation.Id, mediaFile, relisting);
            IsBusy = false;
            EnterCommand.ChangeCanExecute();

            if (!res)
                ShowFailureDialog("Unable to Edit");
            else if (relisting)
                App.Current.MainPage = new RootPage() { Detail = new NavigationPage(new DonationTabPage()) };
            else
                App.Current.MainPage = new RootPage() { Detail = new NavigationPage(new DonationTabPage(donation.Status)) };
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
    }
}
