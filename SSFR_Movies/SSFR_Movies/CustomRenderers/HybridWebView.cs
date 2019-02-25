using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SSFR_Movies.CustomRenderers
{
    public class HybridWebView : View
    {
        public static readonly BindableProperty UriProperty = BindableProperty.Create(
            propertyName: "Uri",
            returnType: typeof(string),
            declaringType: typeof(HybridWebView),
            defaultValue: default(string));

        public EventHandler OnFinishEH;
        public Action<string> Find;
        public string Uri
        {
            get => (string)GetValue(UriProperty);
            set => SetValue(UriProperty, value);
        }

        private bool FindLink(string url)
        {
            return false;
        }

        public void OnFinish()
        {
            OnFinishEH?.Invoke(this, EventArgs.Empty);
        }
    }
}
