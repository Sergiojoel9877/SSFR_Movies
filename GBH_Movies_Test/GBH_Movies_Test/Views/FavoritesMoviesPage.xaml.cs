using CommonServiceLocator;
using GBH_Movies_Test.Data;
using GBH_Movies_Test.Models;
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

            SearchEntry.Focused += SearchEntry_Focused;
            SearchEntry.Unfocused += SearchEntry_Unfocused;

            MoviesList.Focused += MoviesList_Focused;

            Scrollview.Orientation = ScrollOrientation.Horizontal;
        }

        private async void MoviesList_Focused(object sender, FocusEventArgs e)
        {
            
        }

        private async void ItemSelected(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
            {
                return;
            }

            var movie = (Result)e.Item;

            ((ListView)sender).SelectedItem = null;

            if (await DisplayAlert("Suggestion", "Would you like to delete this movie from your favorites list?", "Yes", "No"))
            {
                try
                {
  
                    var deleteMovie = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().DeleteEntity(movie);

                    if (deleteMovie)
                    {
                        await DisplayAlert("Deleted Successfully", "The movie " + movie.Title + " was deleted from your favorite list!", "ok");

                        vm.FavMoviesList.Remove(movie);

                        BindingContext = vm;

                        MoviesList.BeginRefresh();

                        await Task.Delay(500);

                        MoviesList.EndRefresh();
                    }
                }
                catch (Exception)
                {

                }
            };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            ForceLayout();

            BindingContext = null;

            vm.GetStoreMoviesCommand.Execute(null);

            BindingContext = vm;

            MoviesList.BeginRefresh();

            await Task.Delay(500);

            MoviesList.EndRefresh();

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

        private async void SearchEntry_Unfocused(object sender, FocusEventArgs e)
        {
            var t = Scrollview.FadeTo(1, 250, Easing.Linear);

            var t2 = Scrollview.TranslateTo(0, 0, 500, Easing.SpringOut);

            var t3 = SearchFrame.TranslateTo(0, 0, 500, Easing.SpringOut);

            var t4 = MoviesList.TranslateTo(0, 0, 1000, Easing.SpringIn);

            await Task.WhenAll(t, t2, t3, t4);

        }

        private async void SearchEntry_Focused(object sender, FocusEventArgs e)
        {

            var t = Scrollview.FadeTo(0, 500, Easing.Linear);

            var t2 = Scrollview.TranslateTo(2500, 0, 1000, Easing.SpringOut);

            var t3 = SearchFrame.TranslateTo(0, -60, 1000, Easing.SpringOut);

            var t4 = MoviesList.TranslateTo(2500, 0, 1000, Easing.SpringIn);

            await Task.WhenAll(t, t2, t3, t4);

        }
    }
}