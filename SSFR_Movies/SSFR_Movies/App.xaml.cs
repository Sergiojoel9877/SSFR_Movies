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
    /// The main class of a Xamarin Fomrs App.
    /// </summary>
  
    public partial class App : Application
	{
        public static HttpClient httpClient { get; set; } 

        public static DBRepository<Result> DBRepository { get; set; } 

        public static ApiClient ApiClient { get; set; } = new ApiClient();

      
        public App ()
		{
           
            InitializeComponent();

            ContainerInitializer.Initialize();

            //Sets the barrel cache ID.. with out it, the Barrel cannot work
            Barrel.ApplicationId = "SSFR_Movies";
            
            var mainPage = new NavigationPage(new MainPage())
            {
                BarBackgroundColor = Color.FromHex("#272B2E")
            };

            MainPage = mainPage;
    
            SetHttpClient();

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
        private void SetHttpClient() => httpClient = new HttpClient()
                                        {
                                            BaseAddress = new Uri("https://api.themoviedb.org")
                                        };
      
    }
}