using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace OSL.Models
{
    public class DonationCapture
    {
        public string Title { get; set; }
        public ImageSource ImageSource { get; set; }
        public DonationType Type { get; set; }
        public int Quantity { get; set; }
        public DateTime Expiration { get; internal set; }
    }
}
