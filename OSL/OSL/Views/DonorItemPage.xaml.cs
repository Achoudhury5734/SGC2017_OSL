using System;
using Plugin.Media;
using Xamarin.Forms;

namespace OSL
{
    public partial class DonorItemPage : ContentPage
    {
        public Models.Donation Item { get; set; }
        private Image image;

        public DonorItemPage()
        {
            InitializeComponent();
            takePicture.Clicked += Take_Picture;
            image = new Image();
            Item = new Models.Donation()
            {
                Title = "Item name",
                Expiration = DateTime.Now,

            };

            BindingContext = this;
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "AddItem", Item);
            await Navigation.PopToRootAsync();
        }

        async void Take_Picture(object sender, EventArgs e)
        {
			await CrossMedia.Current.Initialize();

			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
			{
				DisplayAlert("No Camera", ":( No camera available.", "OK");
				return;
			}

			var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
			{
				Directory = "Sample",
				Name = "test.jpg"
			});

			if (file == null)
				return;

			await DisplayAlert("File Location", file.Path, "OK");

			image.Source = ImageSource.FromStream(() =>
			{
				var stream = file.GetStream();
				file.Dispose();
				return stream;
			});
        }
    }
}
