
using Android.Content;
using Android.OS;
using Android.Webkit;
using Java.Lang;
using Java.Util.Regex;

/*
 *      xGetter
 *         By
 *   Khun Htetz Naing
 * Repo => https://github.com/KhunHtetzNaing/xGetter
 * Openload,Streamango,StreamCherry,Mp4Upload,RapidVideo,SendVid,VidCloud,MegaUp Stream/Download URL Finder!
 *
 */
namespace SSFR_Movies.Droid.Services
{

    public class XGetter
    {

        private WebView webView;

        private readonly Context context;

        public static OnTaskComplete onComplete;

        private readonly string openload = "https?:\\/\\/(www\\.)?(openload|oload)\\.[^\\/,^\\.]{2,}\\/(embed|f)\\/.+";

        public XGetter(Context view)
        {
            this.context = view;
            this.init();
        }

        private void init()
        {
            this.webView = new WebView(this.context);
            this.webView.SetWillNotDraw(true);
            this.webView.AddJavascriptInterface(new xJavascriptInterface(), "xGetter");
            this.webView.Settings.JavaScriptEnabled = true;
            this.webView.Settings.DomStorageEnabled = true;
            this.webView.SetWebViewClient(new WebViewClient());
        }

        private void letFuck(WebView view)
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

        public void find(string url)
        {

            bool run = false;
            if (this.check(this.openload, url))
            {
                // Openload
                run = true;
            }

            if (run)
            {
                this.webView.LoadUrl(url);
            }
            else
            {
                onComplete.onError();
            }
        }

        private bool check(string regex, string str)
        {
            Java.Util.Regex.Pattern pattern = Java.Util.Regex.Pattern.Compile(regex, RegexOptions.CaseInsensitive);
            Matcher matcher = pattern.Matcher(str);
            return matcher.Find();
        }



        public void onFinish(OnTaskComplete onCompleted)
        {
            onComplete = onCompleted;
        }

    }

    public class xJavascriptInterface : Java.Lang.Object
    {

        [SuppressWarnings()]
        [JavascriptInterface()]
        public void fuck(string url)
        {
            new Handler(Looper.MainLooper).Post(() =>
            {
                XGetter.onComplete.onTaskCompleted(url);
            });
        }
    }

    public interface OnTaskComplete
    {

        void onTaskCompleted(string vidURL);

        void onFbTaskCompleted(string sd, string hd);

        void onError();
    }
}