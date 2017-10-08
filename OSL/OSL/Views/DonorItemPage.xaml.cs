using System;
using Plugin.Media;
using Xamarin.Forms;
using OSL.ViewModels;

namespace OSL.Views
{
    public partial class DonorItemPage : ContentPage
    {
        public DonorItemPage()
        {
            InitializeComponent();
            this.BindingContext = new DonorViewModel
            {
                Page = this
            };
        }
    }
}
