using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonServiceLocator;
using SSFR_Movies.Data;
using SSFR_Movies.Helpers;
using SSFR_Movies.Models;
using SSFR_Movies.Services;
using SSFR_Movies.ViewModels;
using MonkeyCache.FileStore;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.Views
{
    /// <summary>
    /// AllMoviesPage Code Behind
    /// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllMoviesPage : ContentPage
    {
       
        AllMoviesPageViewModel vm;
        
        public AllMoviesPage()
        {
            InitializeComponent();

            activityIndicator.IsRunning = false;

            activityIndicator.IsVisible = false;

            vm = ServiceLocator.Current.GetInstance<ViewModelLocator>().AllMoviesPageViewModel;

            BindingContext = vm;

            SearchEntry.Focused += SearchEntry_Focused;

            SearchEntry.Unfocused += SearchEntry_Unfocused;

            Scrollview.Orientation = ScrollOrientation.Horizontal;

            MoviesList.Focused += MoviesList_Focused;

            MoviesList.Unfocused += MoviesList_Unfocused;

            CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
            
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;

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

            BindingContext = vm;
            
            var t = Scrollview.ScrollToAsync(100, 0, true);

            var t2 = Scrollview.ScrollToAsync(0, 0, true);

            await Task.WhenAll(t, t2);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            BindingContext = null;
            
            GC.Collect();
        }

        private void Current_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        {
            if(e.IsConnected)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    vm.MsgVisible = false;   
                });
                
                MoviesList.BeginRefresh();

                vm.GetStoreMoviesCommand.Execute(null);

                BindingContext = vm;

                MoviesList.EndRefresh();
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    vm.MsgVisible = true;
                });
            }
            
            DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");

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
                    Device.StartTimer(TimeSpan.FromSeconds(1), () =>
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
                        Device.StartTimer(TimeSpan.FromSeconds(1), () =>
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
            catch (Exception e4)
            {
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
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

            var t5 = Search_Icon.ScaleTo(2, 500, Easing.BounceOut);

            var t6 = Search_Icon.ScaleTo(1, 500, Easing.BounceIn);
            
            await Task.WhenAll(t, t2, t3, t4, t5, t6);

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

                var Items = vm.AllMoviesList.Value;

                list.ItemsSource = Items;

                if (Items.Count == 0)
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

                    MoviesList.BeginRefresh();

                    await LoadMoreMovies();

                }
            }
            catch (Exception e1)
            {

                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("An error has occurred!");

                    return false;
                });
            }
        }

        private async Task LoadMoreMovies()
        {
            await Task.Yield();
            try
            {

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

                var MoviesDownloaded = await ServiceLocator.Current.GetInstance<ApiClient>().GetAndStoreMoviesAsync(false, page: Settings.NextPage);

                if (MoviesDownloaded)
                {
                    BindingContext = null;

                    vm.FillUpMoviesListAfterRefreshCommand.Execute(null);

                    if (vm.ListVisible)
                    {
                        BindingContext = vm;

                        Device.BeginInvokeOnMainThread( () =>
                        {
                          
                            MoviesList.EndRefresh();

                            ForceLayout();

                        });
                    }
                }
            }
            catch (Exception e)
            {
                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("An error has ocurred!");

                    return false;
                });
            }
        }

        private async void Genre_Tapped(object sender, EventArgs e)
        {
            await Task.Yield();

            MoviesList.BeginRefresh();

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
              
                var genreType = ((Label)sender).Text;

                var genres = Barrel.Current.Get<Genres>("Genres.Cached");

                var generId = genres.GenresGenres.Where(q => q.Name == genreType).FirstOrDefault().Id;

                var stored = await ServiceLocator.Current.GetInstance<ApiClient>().GetAndStoreMoviesByGenreAsync((int)generId, false);

                if (stored)
                {
                    await MoviesList.TranslateTo(1500, 0, 500, Easing.SpringOut);

                    vm.GetStoreMoviesByGenresCommand.Execute(null);

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        BindingContext = vm;

                        await MoviesList.TranslateTo(0, 0, 500, Easing.SpringIn);

                        MoviesList.EndRefresh();
                        
                        ForceLayout();

                    });
                }
                else
                {
                    MoviesList.EndRefresh();

                    ForceLayout();
                }
            }
            catch (Exception e2)
            {
                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection or maybe that movie doesn't exists!");

                    return false;
                });

                MoviesList.EndRefresh();

                ForceLayout();
            }
        }

        private async void Search_Tapped(object sender, EventArgs e)
        {

            await Task.Yield();


            var key = SearchEntry.Text;

            if (key == "")
            {
                DependencyService.Get<IToast>().LongAlert("The name can't be empty");
                return;
            }

            MoviesList.BeginRefresh();

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

            Device.BeginInvokeOnMainThread(async () =>
            {

                try
                {

                    if (key != "")
                    {
 
                        var movie_results = await ServiceLocator.Current.GetInstance<ApiClient>().SearchMovieByName(key);

                        if (movie_results.Results.Capacity != 0)
                        {

                            vm.AllMoviesList.Value.Clear();

                            foreach (var MovieResult in movie_results.Results)
                            {
                                var PosterPath = "https://image.tmdb.org/t/p/w370_and_h556_bestv2" + MovieResult.PosterPath;

                                var Backdroppath = "https://image.tmdb.org/t/p/w1066_and_h600_bestv2" + MovieResult.BackdropPath;

                                MovieResult.PosterPath = PosterPath;

                                MovieResult.BackdropPath = Backdroppath;

                                vm.AllMoviesList.Value.Add(MovieResult);

                            }

                            BindingContext = vm;

                            await MoviesList.TranslateTo(0, 0, 500, Easing.SpringIn);
                            
                            MoviesList.EndRefresh();

                            var firstItem = vm.AllMoviesList.Value;

                            MoviesList.ScrollTo(firstItem[firstItem.Count - 20], 0, true);

                            ForceLayout();

                        }
                        else
                        {
                            MoviesList.EndRefresh();

                            Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                            {
                                DependencyService.Get<IToast>().LongAlert("It seems like that movie doesn't exists, check your spelling!");

                                return false;
                            });
                        }
                    }
                }
                catch (Exception e3)
                {

                    MoviesList.EndRefresh();

                }

            });
        }
    }
}