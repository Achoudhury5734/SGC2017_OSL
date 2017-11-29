﻿using System;
using System.Collections.Generic;
using OSL.Models;
using Xamarin.Forms;

namespace OSL.Views
{
    public partial class DonorViewCell : ViewCell
    {
        public DonorViewCell()
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
