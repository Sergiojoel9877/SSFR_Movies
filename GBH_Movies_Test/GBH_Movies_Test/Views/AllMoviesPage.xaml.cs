using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonServiceLocator;
using GBH_Movies_Test.Data;
using GBH_Movies_Test.Models;
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
            
            SearchEntry.Focused += SearchEntry_Focused;
            SearchEntry.Unfocused += SearchEntry_Unfocused;

            Scrollview.Orientation = ScrollOrientation.Horizontal;

            MoviesList.Focused += MoviesList_Focused;

            MoviesList.Unfocused += MoviesList_Unfocused;

        }

        private async void MoviesList_Unfocused(object sender, FocusEventArgs e)
        {
            var t = Scrollview.TranslateTo(0, -50, 500, Easing.Linear);

            var t2 = SearchFrame.TranslateTo(0, -100, 500, Easing.Linear);

            var t3 = MoviesList.TranslateTo(0, -50, 500, Easing.Linear);

            await Task.WhenAll(t, t2, t3);
        }

        private async void MoviesList_Focused(object sender, FocusEventArgs e)
        {

            var t = Scrollview.TranslateTo(0, -50, 500, Easing.Linear);

            var t2 = SearchFrame.TranslateTo(0, -100, 500, Easing.Linear);

            var t3 = MoviesList.TranslateTo(0, -50, 500, Easing.Linear);

            await Task.WhenAll(t, t2, t3);
    
        }

        /// <summary>
        /// To animate the Add to Favorite list icon..
        /// </summary>
        /// <param name="sender">the incoming object </param>
        /// <param name="e">the event arguments in that object</param>
        private async void Pin_Tapped(object sender, PanUpdatedEventArgs e)
        {
            var img = sender as Image;
            await img.ScaleTo(2, 500, Easing.BounceOut);
            await img.ScaleTo(1, 250, Easing.BounceIn);
        }

        private async void ItemSelected(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
            {
                return;
            }

            var movie = (Result)e.Item;

            ((ListView)sender).SelectedItem = null;

            if (await DisplayAlert("Suggestion", "Would you like to add this movie to your favorites list?", "Yes", "No"))
            {
                try
                {
                 

                    var movieExists = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().EntityExits(movie.Id);

                    if (movieExists)
                    {
                        await DisplayAlert("Oh no!", "It looks like " + movie.Title + " already exits in your favorite list!", "ok");
                    }

                    var addMovie = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().AddEntity(movie);

                    if (addMovie)
                    {
                      await DisplayAlert("Added Successfully", "The movie " + movie.Title + " was added to your favorite list!", "ok");
                    }
                }
                catch (Exception)
                {
                    
                }
            }
            else
            {
                await Navigation.PushAsync(new MovieDetailsPage(movie));
            }
            
        }

        private async void SearchEntry_Unfocused(object sender, FocusEventArgs e)
        {
            var t = Scrollview.FadeTo(1, 250, Easing.Linear);

            var t2 = Scrollview.TranslateTo(0, 0, 500, Easing.SpringOut);
            
            var t3=  SearchFrame.TranslateTo(0, 0, 500, Easing.SpringOut);

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