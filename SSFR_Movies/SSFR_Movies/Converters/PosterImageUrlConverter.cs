﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace SSFR_Movies.Converters
{
    class PosterImageUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "NoInternet.png";

            return "https://image.tmdb.org/t/p/w370_and_h556_bestv2" + (string)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
