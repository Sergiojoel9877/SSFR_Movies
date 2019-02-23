using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SSFR_Movies.CustomRenderers
{
    public class HybridWebView : View
    {
        Action<string> action;

        public static readonly BindableProperty UriProperty = BindableProperty.Create(
            propertyName: "Uri",
            returnType: typeof(string),
            declaringType: typeof(HybridWebView),
            defaultValue: default(string));

        public string Uri
        {
            get => (string)GetValue(UriProperty);
            set => SetValue(UriProperty, value);
        }

        public void RegisterAction(Action<string> callback)
        {
            action = callback;
        }

        public void CleanUp()
        {
            action = null;
        }
    }
}
