using Android.Content;
using Android.OS;
using Android.Views;
using Android.Webkit;
using SSFR_Movies.CustomRenderers;
using SSFR_Movies.Droid.CustomRenderers;
using SSFR_Movies.Droid.Services;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace SSFR_Movies.Droid.CustomRenderers
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, Android.Webkit.WebView>
    {
        readonly private Context _context;
        //private string org;
        readonly private string openload = "https?:\\/\\/(www\\.)?(openload|oload)\\.[^\\/,^\\.]{2,}\\/(embed|f)\\/.+";

        public HybridWebViewRenderer(Context context) : base(context)
        {
            _context = context;
           
        }

        protected override Android.Webkit.WebView CreateNativeControl()
        {
            return new Android.Webkit.WebView(Context);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null && Control == null)
            {
                var webView = CreateNativeControl();
#pragma warning disable 618 // This can probably be replaced with LinearLayout(LayoutParams.MatchParent, LayoutParams.MatchParent); just need to test that theory
                webView.LayoutParameters = new global::Android.Widget.AbsoluteLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent, 0, 0);
#pragma warning restore 618
                webView.Settings.JavaScriptEnabled = true;
                webView.SetWillNotDraw(true);
                webView.AddJavascriptInterface(new SSFRJavaScriptInterface(), "xGetter");
                webView.Settings.DomStorageEnabled = true;
                webView.SetWebViewClient(new SSFRWebClient());
                webView.Visibility = ViewStates.Visible;
                SetNativeControl(webView);
            }

            //if (e.OldElement != null)
            //{
            //    Control.RemoveJavascriptInterface("xGetter");
            //    var olE = e.OldElement as HybridWebView;
            //    olE.LoadRequest -= OlE_LoadRequest;
            //}
            //if ()
            //{
            //    Control.AddJavascriptInterface(new SSFRJavaScriptInterface(), "xGetter");
            //    var nwE = e.NewElement as HybridWebView;
            //    nwE.LoadRequest += OlE_LoadRequest;
            //}
        }

        //private void OlE_LoadRequest(object sender, HybridWebView.LoadUrlRequested e)
        //{
        //    Control.LoadUrl(e.Url);
        //}

        public class SSFRJavaScriptInterface : Java.Lang.Object
        {
            [JavascriptInterface]
            public void Func(string url)
            {
                new Handler(Looper.MainLooper).Post(() =>
                {
                    //onComplete.OnTaskCompleted(this);
                });
            }
        }

        //public class OnTaskComplete : OnTaskComplete
        //{
        //    public void onError()
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public void onFbTaskCompleted(string sd, string hd)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public void onTaskCompleted(string vidURL)
        //    {
        //        if (vidURL != null)
        //        {
        //            Done(vidURL);
        //        }
        //    }

        //    public void Done(string url)
        //    {
        //        MessagingCenter.Send(this, "URL", url);
        //    }
        //}

        class SSFRWebClient : WebViewClient
        {

            public override void OnLoadResource(Android.Webkit.WebView view, string url)
            {
                base.OnLoadResource(view, url);
            }

            public override void OnPageFinished(Android.Webkit.WebView view, string url)
            {
                base.OnPageFinished(view, url);
                Fuck(view);
            }

            private void Fuck(Android.Webkit.WebView view)
            {
                string encoded = "LyoKICAgICAgICB4R2V0dGVyCiAgICAgICAgICBCeQogICAgS2h1biBIdGV0eiBOYWluZyBbZmIu\n" +
                    "Y29tL0tIdGV0ek5haW5nXQpSZXBvID0+IGh0dHBzOi8vZ2l0aHViLmNvbS9LaHVuSHRldHpOYWlu\n" +
                    "Zy94R2V0dGVyCgoqLwoKdmFyIG9wZW5sb2FkID0gL2h0dHBzPzpcL1wvKHd3d1wuKT8ob3Blbmxv\n" +
                    "YWR8b2xvYWQpXC5bXlwvLF5cLl17Mix9XC8oZW1iZWR8ZilcLy4rL2ksCiAgICBzdHJlYW0gPSAv\n" +
                    "aHR0cHM/OlwvXC8od3d3XC4pPyhzdHJlYW1hbmdvfGZydWl0c3RyZWFtc3xzdHJlYW1jaGVycnl8\n" +
                    "ZnJ1aXRhZGJsb2NrfGZydWl0aG9zdHMpXC5bXlwvLF5cLl17Mix9XC8oZnxlbWJlZClcLy4rL2ks\n" +
                    "CiAgICBtZWdhdXAgPSAvaHR0cHM/OlwvXC8od3d3XC4pPyhtZWdhdXApXC5bXlwvLF5cLl17Mix9\n" +
                    "XC8uKy9pLAogICAgbXA0dXBsb2FkID0gL2h0dHBzPzpcL1wvKHd3d1wuKT9tcDR1cGxvYWRcLlte\n" +
                    "XC8sXlwuXXsyLH1cL2VtYmVkXC0uKy9pLAogICAgc2VuZHZpZCA9IC9odHRwcz86XC9cLyh3d3dc\n" +
                    "Lik/KHNlbmR2aWQpXC5bXlwvLF5cLl17Mix9XC8uKy9pLAogICAgdmlkY2xvdWQgPSAvaHR0cHM/\n" +
                    "OlwvXC8od3d3XC4pPyh2aWRjbG91ZHx2Y3N0cmVhbXxsb2FkdmlkKVwuW15cLyxeXC5dezIsfVwv\n" +
                    "ZW1iZWRcLyhbYS16QS1aMC05XSopL2ksCiAgICByYXBpZHZpZGVvID0gL2h0dHBzPzpcL1wvKHd3\n" +
                    "d1wuKT9yYXBpZHZpZGVvXC5bXlwvLF5cLl17Mix9XC8oXD92PVteJlw/XSp8ZVwvLit8dlwvLisp\n" +
                    "L2k7CgppZiAob3BlbmxvYWQudGVzdCh3aW5kb3cubG9jYXRpb24uaHJlZikpIHsKICAgIHhHZXR0\n" +
                    "ZXIuZnVjayhkb2N1bWVudC5sb2NhdGlvbi5wcm90b2NvbCArICcvLycgKyBkb2N1bWVudC5sb2Nh\n" +
                    "dGlvbi5ob3N0ICsgJy9zdHJlYW0vJyArIGRvY3VtZW50LmdldEVsZW1lbnRCeUlkKCJEdHNCbGtW\n" +
                    "RlF4IikudGV4dENvbnRlbnQgKyAnP21pbWU9dHJ1ZScpOwp9IGVsc2UgaWYgKHN0cmVhbS50ZXN0\n" +
                    "KHdpbmRvdy5sb2NhdGlvbi5ocmVmKSkgewogICAgeEdldHRlci5mdWNrKHdpbmRvdy5sb2NhdGlv\n" +
                    "bi5wcm90b2NvbCArIHNyY2VzWzBdWyJzcmMiXSk7Cn0gZWxzZSBpZiAobWVnYXVwLnRlc3Qod2lu\n" +
                    "ZG93LmxvY2F0aW9uLmhyZWYpKSB7CiAgICBzZWNvbmRzID0gMDsKICAgIGRpc3BsYXkoKTsKICAg\n" +
                    "IHdpbmRvdy5sb2NhdGlvbi5yZXBsYWNlKGRvY3VtZW50LmdldEVsZW1lbnRzQnlDbGFzc05hbWUo\n" +
                    "ImJ0biBidG4tZGVmYXVsdCIpLml0ZW0oMCkuaHJlZik7Cn0gZWxzZSBpZiAobXA0dXBsb2FkLnRl\n" +
                    "c3Qod2luZG93LmxvY2F0aW9uLmhyZWYpKSB7CiAgICB4R2V0dGVyLmZ1Y2soZG9jdW1lbnQuZ2V0\n" +
                    "RWxlbWVudHNCeUNsYXNzTmFtZSgnanctdmlkZW8ganctcmVzZXQnKS5pdGVtKDApLnNyYyk7Cn0g\n" +
                    "ZWxzZSBpZiAocmFwaWR2aWRlby50ZXN0KHdpbmRvdy5sb2NhdGlvbi5ocmVmKSkgewogICAgeEdl\n" +
                    "dHRlci5mdWNrKGRvY3VtZW50LmdldEVsZW1lbnRzQnlUYWdOYW1lKCdzb3VyY2UnKS5pdGVtKDAp\n" +
                    "LnNyYyk7Cn0gZWxzZSBpZiAoc2VuZHZpZC50ZXN0KHdpbmRvdy5sb2NhdGlvbi5ocmVmKSkgewog\n" +
                    "ICAgeEdldHRlci5mdWNrKGRvY3VtZW50LmdldEVsZW1lbnRzQnlUYWdOYW1lKCdzb3VyY2UnKS5p\n" +
                    "dGVtKDApLnNyYyk7Cn0gZWxzZSBpZiAodmlkY2xvdWQudGVzdCh3aW5kb3cubG9jYXRpb24uaHJl\n" +
                    "ZikpIHsKICAgICQuYWpheCh7CiAgICAgICAgdXJsOiAnL2Rvd25sb2FkJywKICAgICAgICBtZXRo\n" +
                    "b2Q6ICdQT1NUJywKICAgICAgICBkYXRhOiB7CiAgICAgICAgICAgIGZpbGVfaWQ6IGZpbGVJRAog\n" +
                    "ICAgICAgIH0sCiAgICAgICAgZGF0YVR5cGU6ICdqc29uJywKICAgICAgICBzdWNjZXNzOiBmdW5j\n" +
                    "dGlvbihyZXMpIHsKICAgICAgICAgICAgJCgnLnF1YWxpdHktbWVudScpLmh0bWwocmVzLmh0bWwp\n" +
                    "OwogICAgICAgICAgICB2YXIgZGF0YSA9IHJlcy5odG1sOwogICAgICAgICAgICB2YXIgcmVnZXgg\n" +
                    "PSAvaHJlZj0iKC4qPykiLzsKICAgICAgICAgICAgdmFyIG07CiAgICAgICAgICAgIGlmICgobSA9\n" +
                    "IHJlZ2V4LmV4ZWMoZGF0YSkpICE9PSBudWxsKSB7CiAgICAgICAgICAgICAgICB4R2V0dGVyLmZ1\n" +
                    "Y2sobVsxXSk7CiAgICAgICAgICAgIH0KICAgICAgICB9CiAgICB9KTsKfSBlbHNlIGlmKHdpbmRv\n" +
                    "dy5sb2NhdGlvbi5ob3N0ID09ICdkcml2ZS5nb29nbGUuY29tJyl7Cglkb2N1bWVudC5nZXRFbGVt\n" +
                    "ZW50QnlJZCgndWMtZG93bmxvYWQtbGluaycpLmNsaWNrKCk7Cn0KCi8qClN1cHBvcnRlZCBTaXRl\n" +
                    "cwo9PiBPcGVubG9hZCAoQWxsIGRvbWFpbnMpCj0+IEZydWl0U3RyZWFtcyAoU3RyZWFtY2hlcnJ5\n" +
                    "LFN0cmVhbWFuZ28gYW5kIGV0Yy4uKQo9PiBNcDRVcGxvYWQKPT4gUmFwaWRWaWRlbwo9PiBTZW5k\n" +
                    "VmlkCj0+IE1lZ2FVcAo9PiBWaWRDbG91ZCAoQWxsIGRvbWFpbnMpCiov";

                view.LoadUrl("javascript:(function){" +
                    "var parent = document.getElementByTagName('head').item(0);" +
                    "var script = document.createElement('script');" +
                    "script.type ='text/jasvascript';" +
                    //Decode the BASE64 using he browser
                    "script.innerHTML = window.atob('" + encoded + "');" +
                    "parent.appendChild(script)" +
                    "})()");
            }
        }

        public void Find(string url, EventArgs e)
        {
            bool run = false;
            if (Check(openload, url))
            {
                //Openload
                run = true;
            }

            if (run)
            {
                Control.LoadUrl(url);
            }
        }

        private bool Check(string openload, string str)
        {
            Java.Util.Regex.Pattern pattern = Java.Util.Regex.Pattern.Compile(openload, Java.Util.Regex.RegexOptions.CaseInsensitive);
            Java.Util.Regex.Matcher matcher = pattern.Matcher(str);
            return matcher.Find();
        }

        //public void OnFinish(OnTaskComplete onTaskCompleted)
        //{
        //    onComplete = onTaskCompleted;
        //}

        public void OnError()
        {
            throw new NotImplementedException();
        }
    }
}