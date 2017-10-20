using OSL.Models;
using OSL.Services;
using OSL.Views;
using Plugin.Media;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Plugin.Media.Abstractions;
using System.Net.Http;

namespace OSL.ViewModels
{
    public class DonationViewModel : ViewModelBase
    {
        private readonly DonationRepository donationRepository;

        public DonationViewModel()
        {
            SaveCommand = new Command(async () => await SaveDonationAsync(), ()=>!IsBusy);
            TakePictureCommand = new Command(async () => await TakePictureAsync());

            ExpirationDate = DateTime.Now;

            //Default to 2 hour expiration
            var expiration = DateTime.Now.AddHours(2);
            ExpirationTime = new TimeSpan(expiration.Hour, expiration.Minute, expiration.Second);

            donationRepository = new DonationRepository();
        }

        private ImageSource imageSource;

        public ImageSource ImageSource
        {
            get { return imageSource; }
            set { SetProperty(ref imageSource, value); }
        }

        private MediaFile mediaFile;

        private string donationTitle;
        public string DonationTitle
        {
            get { return donationTitle; }
            set { SetProperty(ref donationTitle, value); }
        }

        private int quantity;
        public int Quantity
        {
            get { return quantity; }
            set { SetProperty(ref quantity, value); }
        }

        private string donationType;
        public string DonationType
        {
            get { return donationType; }
            set { SetProperty(ref donationType, value); }
        }

        private DateTime expirationDate;
        public DateTime ExpirationDate
        {
            get { return expirationDate; }
            set { SetProperty(ref expirationDate, value); }
        }

        private TimeSpan expirationTime;
        public TimeSpan ExpirationTime
        {
            get { return expirationTime; }
            set { SetProperty(ref expirationTime, value); }
        }

        private Page page;
        public Page Page
        {
            get { return page; }
            set { SetProperty(ref page, value); }
        }

        public Command SaveCommand { get; }
        public ICommand TakePictureCommand { get; }

        public async Task SaveDonationAsync()
        {
            IsBusy = true;
            SaveCommand.ChangeCanExecute();
            await donationRepository.SaveDonationAsync(DonationTitle, mediaFile, Quantity, DonationType, ExpirationDate, ExpirationTime);
            IsBusy = false;
            SaveCommand.ChangeCanExecute();

            await page.Navigation.PushAsync(new DonationListPage());
        }

        public async Task TakePictureAsync()
        {
            await CrossMedia.Current.Initialize();

            //if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            //{
            //    DisplayAlert("No Camera", ":( No camera available.", "OK");
            //    return;
            //}

            mediaFile = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "image.jpg"
            });

            if (mediaFile == null)
                return;

            //await DisplayAlert("File Location", file.Path, "OK");

            ImageSource = ImageSource.FromStream(() =>
            {
                var stream = mediaFile.GetStream();
                //mediaFile.Dispose();
                return stream;
            });

        }
    }
}
