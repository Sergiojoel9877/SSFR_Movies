using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GBH_Movies_Test.Views;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace GBH_Movies_Test
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();

            var mainPage = new NavigationPage(new MainPage()) {

                BarBackgroundColor = Color.FromHex("#272B2E")
            };

            MainPage = mainPage;
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
