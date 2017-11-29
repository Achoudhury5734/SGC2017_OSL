﻿using System;
using OSL.Models;
using Xamarin.Forms;

namespace OSL.Views
{
    public partial class AcceptedViewCell : ViewCell
    {
        public AcceptedViewCell()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            cachedImage.Source = null;
            var item = BindingContext as Donation;

            if (item == null)
                return;

            if (!String.IsNullOrEmpty(item.PictureUrl) && !String.Equals(item.PictureUrl, "Empty"))
                cachedImage.Source = item.PictureUrl;
            
            base.OnBindingContextChanged();
        }
    }
}
