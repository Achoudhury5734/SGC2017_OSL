using System;
using Xamarin.Forms;

namespace OSL.Views
{
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
