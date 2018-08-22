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

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace SSFR_Movies
{
    /// <summary>
    /// The main class of a Xamarin Fomrs App.
    /// </summary>
	public partial class App : Application
	{
        public static HttpClient httpClient { get; set; }

        public App ()
		{
           
            InitializeComponent();
            
            SetHttpClient();
            
            var mainPage = new NavigationPage(new MainPage())
            {
                BarBackgroundColor = Color.FromHex("#272B2E")
            };

            MainPage = mainPage;

        }

		protected override void OnStart ()
		{          
           
		}

		protected override void OnSleep ()
		{
    
		}

		protected override void OnResume ()
		{
            ContainerInitializer.Initialize();
		}

        /// <summary>
        /// Sets the httpClient Baseaddress
        /// </summary>
        private void SetHttpClient() => httpClient = new HttpClient()
                                        {
                                            BaseAddress = new Uri("https://api.themoviedb.org")
                                        };

     

        /// <summary>
        /// Returns an instance of the ViewModelLocator Class
        /// </summary>
        public static ViewModelLocator ViewModelLocator() => new ViewModelLocator();

      
    }
}
