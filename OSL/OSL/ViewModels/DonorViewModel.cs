using OSL.Models;
using OSL.Services;
using Plugin.Media;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace OSL.ViewModels
{
    public class DonorViewModel : ViewModelBase
    {
        private readonly DonationRepository donationRepository;

        public DonorViewModel()
        {
            SaveCommand = new Command(async () => await SaveDonationAsync());
            TakePictureCommand = new Command(async () => await TakePictureAsync());

            donationRepository = new DonationRepository();
        }

        private ImageSource imageSource;

        public ImageSource ImageSource
        {
            get { return imageSource; }
            set { SetProperty(ref imageSource, value); }
        }

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


        public ICommand SaveCommand { get; }
        public ICommand TakePictureCommand { get; }

        public async Task SaveDonationAsync()
        {
            var donationCapture = new DonationCapture
            {
                Quantity = Quantity,
                Expiration = new DateTime(),
                Title = DonationTitle,
                ImageSource = ImageSource,
                Type = (DonationType)Enum.Parse(typeof(DonationType), DonationType)
            };
            await donationRepository.SaveDonationAsync(donationCapture);
        }

        public async Task TakePictureAsync()
        {
            await CrossMedia.Current.Initialize();

            //if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            //{
            //    DisplayAlert("No Camera", ":( No camera available.", "OK");
            //    return;
            //}

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "image.jpg"
            });

            if (file == null)
                return;

            //await DisplayAlert("File Location", file.Path, "OK");

            ImageSource = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });

        }
    }
}
