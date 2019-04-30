using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SSFR_Movies.CustomRenderers
{
    public class HybridWebView : WebView
    {
        public static readonly BindableProperty UriProperty = BindableProperty.Create(
            propertyName: "Uri",
            returnType: typeof(string),
            declaringType: typeof(HybridWebView),
            defaultValue: default(string));

        public event EventHandler<LoadUrlRequested> LoadRequest;

        public event EventHandler OnFinishEH;

        public Action<string> Find;

        public string Uri
        {
            get => (string)GetValue(UriProperty);
            set => SetValue(UriProperty, value);
        }

        public void OnFinish()
        {
            OnFinishEH?.Invoke(this, EventArgs.Empty);
        }

        public void LoadUrl(string url)
        {
            EventHandler<LoadUrlRequested> handler = LoadRequest;
            handler?.Invoke(this, new LoadUrlRequested(url));
        }

        public class LoadUrlRequested
        {
            public string Url { get; set; }

            public LoadUrlRequested(string url)
            {
                this.Url = url;
            }
        }
    }
}
