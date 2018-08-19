using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GBH_Movies_Test.Services;
using GBH_Movies_Test.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GBH_Movies_Test.Views
{
    /// <summary>
    /// AllMoviesPage Code Behind
    /// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AllMoviesPage : ContentPage
	{
        AllMoviesPageViewModel vm;
		public AllMoviesPage ()
		{
			InitializeComponent ();

            vm = ((ViewModelLocator)Application.Current.Resources["Locator"]).AllMoviesPageViewModel;

            BindingContext = vm;

        }
        /// <summary>
        /// To animate the Add to Favorite list icon..
        /// </summary>
        /// <param name="sender">the incoming object </param>
        /// <param name="e">the event arguments in that object</param>
        private async void Pin_Tapped(object sender, EventArgs e)
        {
            var img = sender as Image;
            await img.ScaleTo(2, 500, Easing.BounceOut);
            await img.ScaleTo(1, 250, Easing.BounceIn);
        }

    }
}