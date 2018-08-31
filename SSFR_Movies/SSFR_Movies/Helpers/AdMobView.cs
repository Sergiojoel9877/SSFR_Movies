using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SSFR_Movies.Helpers
{
    public class AdMobView : View
    {
        public static BindableProperty AdUnitIdProperty = BindableProperty.Create(nameof(AdUnitId), typeof(string), typeof(AdMobView), string.Empty);

        public string AdUnitId
        {
            get => (string)GetValue(AdUnitIdProperty);
            set => SetValue(AdUnitIdProperty, value);
        }
    }
}
