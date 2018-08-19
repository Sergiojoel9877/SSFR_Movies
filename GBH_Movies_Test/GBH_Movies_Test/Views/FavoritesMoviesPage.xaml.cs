using GBH_Movies_Test.Services;
using GBH_Movies_Test.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GBH_Movies_Test.Views
{
    /// <summary>
    /// FavoriteMoviesPage Code Behind
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FavoritesMoviesPage : ContentPage
	{
        FavoriteMoviesPageViewModel vm;

		public FavoritesMoviesPage ()
		{
			InitializeComponent ();

            vm = ((ViewModelLocator)Application.Current.Resources["Locator"]).FavoriteMoviesPageViewModel;

            BindingContext = vm;
		}

        /// <summary>
        /// To animate the Quit form Favorite list icon..
        /// </summary>
        /// <param name="sender">the incoming object </param>
        /// <param name="e">the event arguments in that object</param>
        private async void UnPin_Tapped(object sender, EventArgs e)
        {
            var img = sender as Image;
            await img.ScaleTo(2, 500, Easing.BounceOut);
            await img.ScaleTo(1, 250, Easing.BounceIn);
        }
        
    }
}