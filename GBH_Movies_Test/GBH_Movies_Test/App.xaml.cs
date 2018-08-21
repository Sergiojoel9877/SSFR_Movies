using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GBH_Movies_Test.Views;
using GBH_Movies_Test.Services;
using System.Threading.Tasks;
using CommonServiceLocator;
using System.Net.Http;
using MonkeyCache.FileStore;
using GBH_Movies_Test.Helpers;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace GBH_Movies_Test
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
            Settings.NextPage = 1;

            Settings.NextPageForGenre = 1;
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
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
