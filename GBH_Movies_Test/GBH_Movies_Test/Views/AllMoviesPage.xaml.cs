using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonServiceLocator;
using GBH_Movies_Test.Data;
using GBH_Movies_Test.Helpers;
using GBH_Movies_Test.Models;
using GBH_Movies_Test.Services;
using GBH_Movies_Test.ViewModels;
using MonkeyCache.FileStore;
using Plugin.Connectivity;
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
        bool IsLoading;

        AllMoviesPageViewModel vm;

        public AllMoviesPage()
        {
            InitializeComponent();

            activityIndicator.IsRunning = false;

            activityIndicator.IsVisible = false;

            vm = ((ViewModelLocator)Application.Current.Resources["Locator"]).AllMoviesPageViewModel;

            BindingContext = vm;

            SearchEntry.Focused += SearchEntry_Focused;

            SearchEntry.Unfocused += SearchEntry_Unfocused;

            Scrollview.Orientation = ScrollOrientation.Horizontal;

            MoviesList.Focused += MoviesList_Focused;

            MoviesList.Unfocused += MoviesList_Unfocused;

        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            await Scrollview.ScrollToAsync(100, 0, true);

            await Scrollview.ScrollToAsync(0, 0, true);
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
            try
            {

                if (e.Item == null)
                {
                    return;
                }

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
            catch (Exception)
            {
                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection or maybe that movie doesn't exists!");

                    return false;
                });
            }
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

        private async void MoviesList_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {

            try
            {
                var list = (ListView)sender;

                var Items = vm.AllMoviesList;

                list.ItemsSource = Items;

                if (IsLoading || Items.Count == 0)
                {
                    return;
                }

                if (e.Item == Items[Items.Count - 1])
                {
                    Settings.NextPage++;

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

                    await LoadMoreMovies();

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

        private async Task LoadMoreMovies()
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

            await Task.Yield();

            var t = MoviesList.TranslateTo(0, -60, 250, Easing.Linear);

            var t2 = Scrollview.TranslateTo(0, -60, 250, Easing.Linear);

            var t3 = SearchFrame.TranslateTo(0, -60, 250, Easing.Linear);

            await Task.WhenAll(t, t2, t3);

            activityIndicator.IsVisible = true;

            activityIndicator.IsRunning = true;

            var MoviesDownloaded = await ServiceLocator.Current.GetInstance<ApiClient>().GetAndStoreMoviesAsync(false, page: Settings.NextPage);

            if (MoviesDownloaded)
            {
                BindingContext = null;

                vm.FillUpMoviesListAfterRefreshCommand.Execute(null);

                if (vm.ListVisible)
                {
                    BindingContext = vm;

                    MoviesList.BeginRefresh();

                    await Task.Delay(2000);

                    MoviesList.EndRefresh();
                    
                    activityIndicator.IsVisible = false;

                    activityIndicator.IsRunning = false;

                    var t4 = MoviesList.TranslateTo(0, 0, 250, Easing.Linear);

                    var t5 = Scrollview.TranslateTo(0, 0, 250, Easing.Linear);

                    var t6 = SearchFrame.TranslateTo(0, 0, 250, Easing.Linear);

                    await Task.WhenAll(t4, t5, t6);
                } 
            }
        }

        private async void Genre_Tapped(object sender, EventArgs e)
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

            try
            {
                await Task.Yield();

                var genreType = ((Label)sender).Text;

                var genres = Barrel.Current.Get<Genres>("Genres.Cached");

                var generId = genres.GenresGenres.Where(q => q.Name == genreType).FirstOrDefault().Id;

                var stored = await ServiceLocator.Current.GetInstance<ApiClient>().GetAndStoreMoviesByGenreAsync((int)generId, false);

                if (stored)
                {
                    await MoviesList.TranslateTo(1500, 0, 500, Easing.SpringOut);

                    activityIndicator.IsVisible = true;

                    activityIndicator.IsRunning = true;

                    ForceLayout();

                    vm.GetStoreMoviesByGenresCommand.Execute(null);

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        BindingContext = vm;

                        MoviesList.BeginRefresh();

                        await MoviesList.TranslateTo(0, 0, 500, Easing.SpringIn);

                        await Task.Delay(2000);

                        MoviesList.EndRefresh();

                        activityIndicator.IsVisible = false;

                        activityIndicator.IsRunning = false;
                    });
                }
                else
                {
                    DependencyService.Get<IToast>().LongAlert("An has ocurred!");
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

        private async void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
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

            await Task.Yield();

            var key = e.NewTextValue;

            if (key != "")
            {
                try
                {
                    await MoviesList.TranslateTo(1500, 0, 500, Easing.SpringOut);

                    activityIndicator.IsVisible = true;

                    activityIndicator.IsRunning = true;

                    var movie_results = await ServiceLocator.Current.GetInstance<ApiClient>().SearchMovieByName(key);

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        vm.AllMoviesList.Clear();

                        vm.AllMoviesList.Add(movie_results.Results.FirstOrDefault());

                        foreach (var MovieResult in movie_results.Results)
                        {
                            var PosterPath = "https://image.tmdb.org/t/p/w370_and_h556_bestv2" + MovieResult.PosterPath;

                            var Backdroppath = "https://image.tmdb.org/t/p/w1066_and_h600_bestv2" + MovieResult.BackdropPath;

                            MovieResult.PosterPath = PosterPath;

                            MovieResult.BackdropPath = Backdroppath;

                            vm.AllMoviesList.Add(MovieResult);

                        }

                        BindingContext = vm;

                        MoviesList.BeginRefresh();

                        await MoviesList.TranslateTo(0, 0, 500, Easing.SpringIn);

                        await Task.Delay(2000);

                        MoviesList.EndRefresh();

                        activityIndicator.IsVisible = false;

                        activityIndicator.IsRunning = false;
                    });

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
}