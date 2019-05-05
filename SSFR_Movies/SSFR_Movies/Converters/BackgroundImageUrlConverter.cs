using System;
using System.Globalization;
using Xamarin.Forms;

namespace SSFR_Movies.Converters
{
    class BackgroundImageUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var URL = "https://image.tmdb.org/t/p/w1066_and_h600_bestv2";

            return URL + (string)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
