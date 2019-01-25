using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SSFR_Movies.Views;
using SSFR_Movies.Services;
using System.Threading.Tasks;
using CommonServiceLocator;
using System.Net.Http;
using MonkeyCache.FileStore;
using SSFR_Movies.Helpers;
using System.Collections;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Collections.Generic;
using Xamarin.Forms.Internals;
using SSFR_Movies.Data;
using SSFR_Movies.Models;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace SSFR_Movies
{
    /// <summary>
    /// The main class of a Xamarin Forms App.
    /// </summary>
    [Preserve(AllMembers = true)]
    public partial class App : Application
	{
        public static Lazy<HttpClient> httpClient { get; set; }
      
        public App ()
		{
            InitializeComponent();

            var mainPage = new Lazy<NavigationPage>(() => new NavigationPage(new StartPagexaml()));

            MainPage = mainPage.Value;

            //var mainPage = new NavigationPage(new MainPage())
            //{
            //    BarBackgroundColor = Color.FromHex("#272B2E")
            //};
            //var mainPage = new StartPagexaml();

            //MainPage = mainPage;
        }

        async void InitializeAsync(Func<Task> action)
        {
            await action();
        }

        protected override void OnStart ()
		{
            AppCenter.Start("android=8d9e8fc5-562a-434b-934c-cd959dc47068;", typeof(Analytics), typeof(Crashes));
        }

		protected override void OnSleep ()
		{
            
		}

		protected override void OnResume ()
		{
          
		}

        /// <summary>
        /// Sets the httpClient Baseaddress
        /// </summary>
        private void SetHttpClient() => httpClient = new Lazy<HttpClient>(() => new HttpClient()
        {
            BaseAddress = new Uri("https://api.themoviedb.org")
        });


    }
}
