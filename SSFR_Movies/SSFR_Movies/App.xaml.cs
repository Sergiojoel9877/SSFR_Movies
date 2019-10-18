using SSFR_Movies.Views;
using System;
using System.Net.Http;
using Xamarin.Forms;
//using Microsoft.AppCenter;
//using Microsoft.AppCenter.Analytics;
//using Microsoft.AppCenter.Crashes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
//using SSFR_Movies.Data;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SSFR_Movies
{
    /// <summary>
    /// The main class of a Xamarin Forms App.
    /// </summary>
    [Preserve(AllMembers = true)]
    public partial class App : Application
    {
        public static HttpClient HttpClient { get; set; }

        public App()
        {
            InitializeComponent();

            ///XF.Material.Forms.Material.Init(this);

#if DEBUG
            //HotReloader.Current.Run(this, new HotReloader.Configuration()
            //{
            //    PreviewerDefaultMode = HotReloader.PreviewerMode.On
            //});
#endif

            SetMainPage();

            SetHttpClient();
        }

        private void SetMainPage()
        {
            //new Lazy<ContainerInitializer>(() => new ContainerInitializer()).Value.Initialize();
            //MainPage = new Lazy<AppShell>(() => new AppShell()).Value;
            MainPage = new Lazy<Start>(() => new Start()).Value;
        }

        protected override void OnStart()
        {
            //AppCenter.Start("android=8d9e8fc5-562a-434b-934c-cd959dc47068;", typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {

        }

        protected override void OnResume()
        {

        }

        /// <summary>
        /// Sets the httpClient Baseaddress
        /// </summary>
        private void SetHttpClient() => HttpClient = new HttpClient()
        {
            BaseAddress = new Uri("https://api.themoviedb.org")
        };
    }
}
