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
using SSFR_Movies.Helpers;
using Xamarin.Forms.Internals;

namespace SSFR_Movies.Views
{
    /// <summary>
    /// FavoriteMoviesPage Code Behind
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FavoritesMoviesPage : ContentPage
	{
        FavoriteMoviesPageViewModel vm;
        
        ToolbarItem searchToolbarItem = null;

        public FavoritesMoviesPage()
		{
			InitializeComponent();

            vm = ServiceLocator.Current.GetInstance<Lazy<FavoriteMoviesPageViewModel>>().Value;

            BindingContext = vm;

            SetVisibility();

            searchToolbarItem = new ToolbarItem()
            {
                Text = "Search",
                Icon = "Search.png",
                Priority = 0,

                Command = new Command(async () =>
                {
                    await Navigation.PushAsync(new SearchPage(), true);
                })
            };
                       
            ToolbarItems.Add(searchToolbarItem);

            MoviesList.SelectionChangedCommand = new Command(MovieSelected);

            SubscribeToMessage();
        }

        private void SubscribeToMessage()
        {
            MessagingCenter.Subscribe<MovieDetailsPage, bool>(this, "Refresh", (s, e) =>
            {
                if (e)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                    var estado = await vm.FillMoviesList(null);
                        if (estado.Key == 'v')
                        {
                            //UnPin.IsVisible = true;
                            //Message.IsVisible = true;
                            MoviesList.IsVisible = true;
                            UnPin.IsVisible = false;
                            Message.IsVisible = false;
                            MoviesList.ItemsSource = estado.Value;
                            MoviesList.SelectedItem = null;
                        }
                        else if(estado.Key == 'r')
                        {
                            MoviesList.IsVisible = true;
                            UnPin.IsVisible = false;
                            Message.IsVisible = false;
                            MoviesList.ItemsSource = estado.Value;
                            MoviesList.SelectedItem = null;
                        }
                    });
                }
            });

            MessagingCenter.Subscribe<CustomViewCellFavPage, bool>(this, "Refresh", (s, e) =>
            {
                if (e)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        var estado = await vm.FillMoviesList();
                        if (estado == 'v')
                        {
                            UnPin.IsVisible = true;
                            Message.IsVisible = true;
                        }
                        else if (estado == 'r')
                        {
                            MoviesList.IsVisible = true;
                            UnPin.IsVisible = false;
                            Message.IsVisible = false;
                        }
                    });
                }
            });

            MessagingCenter.Subscribe<CustomViewCellFavPage>(this, "PushAsync", (e) =>
            {
                MovieSelected();
            });

            //MessagingCenter.Subscribe<MovieDetailsPage>(this, "ClearSelection", (e) =>
            //{
            //    MoviesList.SelectedItem = null;
            //});
        }

        private void T_Tapped(object sender, EventArgs e)
        {
            MovieSelected();
        }

        private async void MovieSelected()
        {
            if (MoviesList.SelectedItem != null)
            {
                var movie = MoviesList.SelectedItem as Result;
                MoviesList.SelectedItem = null;
                await Navigation.PushAsync(new MovieDetailsPage(movie));
            }
        }

        private async void QuitFromFavorites(object sender, EventArgs e)
        {


            if (sender is MenuItem opt)
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

                if (await DisplayAlert("Suggestion", "Would you like to delete this movie from your favorites list?", "Yes", "No"))
                {
                    try
                    {
                        var deleteMovie = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().DeleteEntity(movie);

                        if (deleteMovie)
                        {
                            await DisplayAlert("Deleted Successfully", "The movie " + movie.Title + " was deleted from your favorite list!", "ok");

                            vm.FavMoviesList.Value.Remove(movie);

                            BindingContext = vm;

                            await Task.Delay(500);

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
            
            if (e.Item == null)
            {
                return;
            }

            var movie = (Result)e.Item;

            ((ListView)sender).SelectedItem = null;

            await Navigation.PushAsync(new MovieDetailsPage(movie));

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
    }
}
 