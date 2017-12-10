using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OSL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PickupViewCell : ViewCell
    {
        public PickupViewCell()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            cachedImage.Source = null;
            var item = BindingContext as PickupItem;

            if (item == null)
                return;

            if (!String.IsNullOrEmpty(item.PictureUrl) && !String.Equals(item.PictureUrl, "Empty"))
                cachedImage.Source = item.PictureUrl;
            
            base.OnBindingContextChanged();
        }
    }
}
