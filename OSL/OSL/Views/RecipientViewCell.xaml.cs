using System;
using Xamarin.Forms;

namespace OSL.Views
{
    public partial class RecipientViewCell : ViewCell
    {
        public RecipientViewCell()
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
