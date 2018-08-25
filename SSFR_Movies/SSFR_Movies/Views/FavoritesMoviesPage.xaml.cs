using CommonServiceLocator;
using SSFR_Movies.Data;
using SSFR_Movies.Models;
using SSFR_Movies.Services;
using SSFR_Movies.ViewModels;
using MonkeyCache.FileStore;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.Views
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

            //vm = ((ViewModelLocator)Application.Current.Resources["Locator"]).FavoriteMoviesPageViewModel;

            vm = ServiceLocator.Current.GetInstance<ViewModelLocator>().FavoriteMoviesPageViewModel;

            BindingContext = vm;

            SetVisibility();

        }

        private async void AddToFavList(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                MoviesList.IsEnabled = false;
            });

            var opt = sender as MenuItem;

            if (opt != null)
            {

                var movie = opt.BindingContext as Result;

                //Verify if internet connection is available
                if (!CrossConnectivity.Current.IsConnected)
                {
                    Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                    {
                        DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");
                        return false;
                    });
                    return;
                }

                if (await DisplayAlert("Suggestion", "Would you like to add this movie to your favorites list?", "Yes", "No"))
                {
                    try
                    {
                        var movieExists = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().EntityExits(movie.Id);

                        if (movieExists)
                        {
                            await DisplayAlert("Oh no!", "It looks like " + movie.Title + " already exits in your favorite list!", "ok");

                         
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                MoviesList.IsEnabled = true;
                            });
                        }

                        var addMovie = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().AddEntity(movie);

                        if (addMovie)
                        {
                            await DisplayAlert("Added Successfully", "The movie " + movie.Title + " was added to your favorite list!", "ok");

                            Device.BeginInvokeOnMainThread(() =>
                            {
                                MoviesList.IsEnabled = true;
                            });
                        }
                    }
                    catch (Exception)
                    {
                        Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                        {
                            DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection or maybe that movie doesn't exists!");

                            Device.BeginInvokeOnMainThread(() =>
                            {
                                MoviesList.IsEnabled = true;
                            });

                            return false;
                        });
                    }
                }
                else
                {
                    
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        MoviesList.IsEnabled = true;
                    });
                }
            }
        }


        private async void SetVisibility()
        {
            var movies_db = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().GetEntities();

            UnPin.IsVisible = movies_db.Count() == 0 ? true : false;

            Message.IsVisible = UnPin.IsVisible == true ? true : false;

            MoviesList.IsVisible = Message.IsVisible == true ? false : true;
        }

        private async void QuitVisibility()
        {
            var movies_db = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().GetEntities();

            UnPin.IsVisible = movies_db.Count() != 0 ? false : true;

            Message.IsVisible = UnPin.IsVisible == true ? true : false;

            MoviesList.IsVisible = Message.IsVisible == true ? false : true;

        }

        private async void ItemSelected(object sender, ItemTappedEventArgs e)
        {
            //Verify if internet connection is available
            if (!CrossConnectivity.Current.IsConnected)
            {
                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");
                    return false;
                });
                return;
            }

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

                        var moviesRemaining = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().GetEntities();

                        if (moviesRemaining.Count() == 0)
                        {
                            QuitVisibility();
                        }
                    }
                }
                catch (Exception)
                {
                    Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                    {
                        DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection or maybe that movie doesn't exists!");

                        return false;
                    });
                }
            }
            else
            {
                await Navigation.PushAsync(new MovieDetailsPage(movie));
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var movies_db = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().GetEntities();

            UnPin.IsVisible = movies_db.Count() == 0 ? true : false;

            Message.IsVisible = UnPin.IsVisible == true ? true : false;

            MoviesList.IsVisible = Message.IsVisible == true ? false : true;
    
            ForceLayout();
            
            vm.GetStoreMoviesCommand.Execute(null);

            BindingContext = vm;

            MoviesList.BeginRefresh();

            await Task.Delay(500);

            MoviesList.EndRefresh();

        }

        /// <summary>
        /// To animate the Quit from Favorite list icon..
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
            var t = MoviesList.TranslateTo(0, 0, 1000, Easing.SpringIn);

            await Task.WhenAll(t);

        }

        private async void SearchEntry_Focused(object sender, FocusEventArgs e)
        {

            var t = MoviesList.TranslateTo(2500, 0, 1000, Easing.SpringIn);

            await Task.WhenAll(t);

        }
    }
}