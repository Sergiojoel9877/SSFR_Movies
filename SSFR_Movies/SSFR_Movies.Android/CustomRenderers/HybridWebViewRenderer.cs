using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SSFR_Movies.Droid.CustomRenderers;
using SSFR_Movies.Helpers;
using Xamarin.Forms;
using SSFR_Movies.Droid;
using Xamarin.Forms.Platform.Android;
using SSFR_Movies.CustomRenderers;
using Android.Webkit;
using System.Threading.Tasks;
using Java.Lang;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace SSFR_Movies.Droid.CustomRenderers
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, Android.Webkit.WebView>
    {
        private Context _context;
        public OnTaskCompleted onComplete;
            
        public HybridWebViewRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var webView = new Android.Webkit.WebView(_context);
                webView.Settings.JavaScriptEnabled = true;
                webView.SetWillNotDraw(true);
                webView.AddJavascriptInterface(new SSFRJavaScriptInterface(), "SSFR");
                webView.Settings.DomStorageEnabled = true;
                var vebClient = new WebViewClient()
                {

                };
                webView.SetWebViewClient(new WebViewClient()
                {
                    public override void OnLoadResource(Android.Webkit.WebView, string url)
                    {
                        base
                    }
                });
            }
        }
        class SSFRJavaScriptInterface : Java.Lang.Object
        {
            [JavascriptInterface]
            public void func(string url)
            {
                new Handler(Looper.MainLooper).Post(() =>
                {
                    
                });
            }
        }
    }

    public interface OnTaskCompleted
    {
        void onTaskCompleted(string vidUrl);
        void onError();
    }

    
}