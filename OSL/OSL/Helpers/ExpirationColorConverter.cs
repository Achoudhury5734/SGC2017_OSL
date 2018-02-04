using System;
using System.Globalization;
using OSL.Models;
using Xamarin.Forms;

namespace OSL.Helpers
{
    public class ExpirationColorConverter :IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Color.Default;

            DateTime expiration;
            if (value.GetType() == typeof(Donation))
            {
                var donation = (Donation)value;
                if (donation.Status == DonationStatus.Completed || !donation.Expiration.HasValue)
                    return Color.Default;
                
                expiration = donation.Expiration.Value;
            }
            else
            {
                expiration = (DateTime)value;
            }
 
            if (expiration < DateTime.Now)
                return Color.FromHex("#e33033"); //same red as chart, sampled from apple logo
            else
                return Color.Default;
        }

        // Not used. Converter is just for one way bindings
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
