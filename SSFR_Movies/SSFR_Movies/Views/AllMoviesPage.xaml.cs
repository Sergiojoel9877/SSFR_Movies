using AsyncAwaitBestPractices;
using Plugin.SharedTransitions;
using Realms;
using Splat;
using SSFR_Movies.CustomRenderers;
using SSFR_Movies.Helpers;
using SSFR_Movies.Models;
using SSFR_Movies.Services;
using SSFR_Movies.ViewModels;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.Material.Forms.UI.Dialogs;
using XF.Material.Forms.UI.Dialogs.Configurations;

namespace SSFR_Movies.Views
{
    /// <summary>
    /// AllMoviesPage Code Behind
    /// </summary>
    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllMoviesPage : ContentPage
    {
        readonly AllMoviesPageViewModel vm = null;

        FlexLayout genresContainer = null;

        ToolbarItem updownList = null;

        ToolbarItem searchToolbarItem = null;

        PullToRefreshLayout pull2refreshlyt = null;

        readonly MaterialSnackbarConfiguration _conf = new MaterialSnackbarConfiguration()
        {
            TintColor = Color.FromHex("#0066cc"),
            BackgroundColor = Color.FromHex("#272B2E")
        };

        public AllMoviesPage()
        {
            InitializeComponent();

            vm = Locator.Current.GetService<AllMoviesPageViewModel>();

            BindingContext = vm;

            SetPullToRefresh();

            SetPull2RefreshToMainStack();
            
            SuscribeToMessages();

            SetContainerForMovieGenres().SafeFireAndForget();

            SetToolBarItems();

            SetScrollViewOrientation();
        }

        private void SetScrollViewOrientation()
        {
            Scrollview.Orientation = ScrollOrientation.Horizontal;
        }

        private void SetToolBarItems()
        {
            updownList = new ToolbarItem()
            {
                Text = "Up",
                Icon = "ListDown.png",
                Priority = 1,
                Command = new Command(async () =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        updownList.Icon = updownList.Icon == "ListDown.png" ? "ListUp.png" : "ListDown.png";
                    });

                    if (updownList.Icon == "ListDown.png")
                    {
                        if (MainThread.IsMainThread)
                        {
                            genresContainer.IsVisible = false;

                            await Scrollview.TranslateTo(0, -80, 150, Easing.Linear);
                        }
                        else
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                genresContainer.IsVisible = false;

                                await Scrollview.TranslateTo(0, -80, 150, Easing.Linear);
                            });
                        }
                    }
                    else
                    {
                        if (MainThread.IsMainThread)
                        {
                            genresContainer.IsVisible = true;

                            await Scrollview.TranslateTo(0, 0, 150, Easing.Linear);
                        }
                        else
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                genresContainer.IsVisible = true;

                                await Scrollview.TranslateTo(0, 0, 150, Easing.Linear);
                            });
                        }
                    }
                })
            };

            searchToolbarItem = new ToolbarItem()
            {
                Text = "Search",
                Icon = "Search.png",
                Priority = 0,

                Command = new Command(async () =>
                {
                    if (MainThread.IsMainThread)
                    {
                        await Shell.Current.GoToAsync("app://ssfr.com/Search", true);
                    }
                    else
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await Shell.Current.GoToAsync("app://ssfr.com/Search", true);
                        });
                    }
                })
            };

            ToolbarItems.Add(updownList);

            ToolbarItems.Add(searchToolbarItem);

        }

        private async Task<FlexLayout> SetContainerForMovieGenres()
        {
            genresContainer = this.FindByName<FlexLayout>("GenresContainer");

            if (MainThread.IsMainThread)
            {
                genresContainer.IsVisible = false;
                await Scrollview.TranslateTo(0, -80, 500, Easing.Linear);
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    genresContainer.IsVisible = false;
                    await Scrollview.TranslateTo(0, -80, 500, Easing.Linear);
                });
            }
            return genresContainer;
        }

        private void SetPull2RefreshToMainStack() => stack.Children.Add(pull2refreshlyt);

        private PullToRefreshLayout SetPullToRefresh()
        {
            pull2refreshlyt = Locator.Current.GetService<PullToRefreshLayout>();
            pull2refreshlyt.Content = scroll;
            pull2refreshlyt.RefreshBackgroundColor = Color.FromHex("#272B2E");
            pull2refreshlyt.RefreshColor = Color.FromHex("#006FDE");
            pull2refreshlyt.SetBinding(PullToRefreshLayout.RefreshCommandProperty, "FillUpMoviesListAfterRefreshCommand");
            pull2refreshlyt.RefreshCommand = new Command(async () =>
            {
                await LoadMoreMovies();
            });

            return pull2refreshlyt;
        }

        private void MovieSelected(object sender, SelectionChangedEventArgs e)
        {
            if (MoviesList.SelectedItem != null)
            {
                var movie = MoviesList.SelectedItem as Result;

                Result resultSingleton = ResultSingleton.SetInstance(movie);

                if (MainThread.IsMainThread)
                {
                    SharedTransitionNavigationPage.SetSelectedTagGroup(this, movie.Id);
                    Shell.Current.GoToAsync("app://ssfr.com/MovieDetails", true).SafeFireAndForget();
                }
                else
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Shell.Current.GoToAsync("app://ssfr.com/MovieDetails", true).SafeFireAndForget();
                        //await Navigation.PushAsync(new MovieDetailsPage(movie));
                    });
                }
            }
        }

        private void SuscribeToMessages()
        {
            MessagingCenter.Subscribe<MovieDetailsPage>(this, "ClearSelection", (e) =>
            {
                MoviesList.SelectedItem = null;
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            SuscribeToMessages();

            Connectivity.ConnectivityChanged += Current_ConnectivityChanged;

            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    vm.NoNetWorkHideTabs.Execute(null);

                    Shell.SetTitleView(this, null);

                    var titleView = new StackLayout()
                    {
                        HorizontalOptions = LayoutOptions.Start,
                        Orientation = StackOrientation.Horizontal
                    };

                    titleView.Children.Add(new Label()
                    {
                        Text = "Connecting...",
                        TextColor = Color.White,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 20,
                        VerticalTextAlignment = TextAlignment.Center
                    });

                    titleView.Children.Add(new ActivityIndicator()
                    {
                        Color = Color.White,
                        HeightRequest = 25,
                        WidthRequest = 25,
                        VerticalOptions = LayoutOptions.Center,
                        IsRunning = true
                    });
                    Shell.SetTitleView(this, titleView);
                    Shell.SetTabBarIsVisible(this, false);
                });
                return;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            Connectivity.ConnectivityChanged -= Current_ConnectivityChanged;
        }

        private void Current_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
            {
                Shell.SetTitleView(this, null);
                Shell.SetTitleView(this, new Label()
                {
                    Text = "SSFR Movies",
                    TextColor = Color.White,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 20,
                    VerticalTextAlignment = TextAlignment.Center
                });
                Shell.SetTabBarIsVisible(this, true);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    vm.ListVisible = true;
                });

                vm.GetStoreMoviesCommand.Execute(null);

                BindingContext = vm;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MoviesList.ItemsSource = null;
                    MoviesList.ItemsSource = vm.AllMoviesList.Value;
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    vm.NoNetWorkHideTabs.Execute(null);
                    Shell.SetTitleView(this, null);

                    var titleView = new StackLayout()
                    {
                        HorizontalOptions = LayoutOptions.Start,
                        Orientation = StackOrientation.Horizontal
                    };

                    titleView.Children.Add(new Label()
                    {
                        Text = "Connecting...",
                        TextColor = Color.White,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 20,
                        VerticalTextAlignment = TextAlignment.Center
                    });

                    titleView.Children.Add(new ActivityIndicator()
                    {
                        Color = Color.White,
                        HeightRequest = 25,
                        WidthRequest = 25,
                        VerticalOptions = LayoutOptions.Center,
                        IsRunning = true
                    });
                    Shell.SetTitleView(this, titleView);
                    Shell.SetTabBarIsVisible(this, false);
                });
            }
        }

        private async Task LoadMoreMovies()
        {
            await Task.Yield();

            try
            {
                //Verify if internet connection is available
                if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
                {
                    pull2refreshlyt.IsRefreshing = false;
                    await MaterialDialog.Instance.SnackbarAsync("No internet Connection", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf);
                    return;
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    //activityIndicator.IsRunning = true;
                    //activityIndicator.IsVisible = true;
                    MoviesList.IsVisible = false;
                    RefreshBtn.IsVisible = false;
                });

                Settings.NextPage++;

                vm.AllMoviesList.Value.Clear();

                var token = new CancellationTokenSource();

                token.CancelAfter(4000);

                //var MoviesDownloaded = await ServiceLocator.Current.GetInstance<Lazy<ApiClient>>().Value.GetAndStoreMoviesAsync(false, page: Settings.NextPage);
                var MoviesDownloaded = await Locator.Current.GetService<ApiClient>().GetAndStoreMoviesAsync(false, page: Settings.NextPage);

                if (MoviesDownloaded)
                {
                    BindingContext = null;

                    vm.FillUpMoviesListAfterRefreshCommand.Execute(null);

                    if (vm.ListVisible)
                    {
                        BindingContext = vm;

                        MainThread.BeginInvokeOnMainThread(() =>
                       {
                            //pull2refreshlyt.IsRefreshing = false;
                            //activityIndicator.IsRunning = false;
                            MoviesList.IsVisible = true;
                           activityIndicator.IsVisible = false;
                           RefreshBtn.IsVisible = true;
                           pull2refreshlyt.IsRefreshing = false;
                       });
                    }
                }
            }
            catch (Exception e)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    pull2refreshlyt.IsRefreshing = false;
                    //activityIndicator.IsRunning = false;
                    MoviesList.IsVisible = true;
                    activityIndicator.IsVisible = false;
                    RefreshBtn.IsVisible = true;
                });

                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    Task.Run(async () =>
                    {
                        await MaterialDialog.Instance.SnackbarAsync("An error has ocurred!", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf);
                    });

                    Debug.WriteLine("Error: " + e.InnerException);
                    return false;
                });
            }
        }

        private async void Genre_Tapped(object sender, EventArgs e)
        {
            await Task.Yield();

            //Verify if internet connection is available
            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                await MaterialDialog.Instance.SnackbarAsync("An error has ocurred!", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf);

                return;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                vm.ListVisible = false;
                vm.IsEnabled = true;
                vm.IsRunning = true;
            });

            MoviesList.ItemsSource = null;

            try
            {
                var realm = await Realm.GetInstanceAsync();

                var genreType = ((Label)sender).Text;

                var genres = realm.All<Genres>().FirstOrDefault();

                var generId = genres.GenresGenres.Where(q => q.Name == genreType).FirstOrDefault().Id;

                //var stored = await ServiceLocator.Current.GetInstance<Lazy<ApiClient>>().Value.GetAndStoreMoviesByGenreAsync((int)generId, false);
                var stored = await Locator.Current.GetService<ApiClient>().GetAndStoreMoviesByGenreAsync(generId, false);

                if (stored)
                {
                    await MoviesList.TranslateTo(1500, 0, 500, Easing.SpringOut);

                    vm.GetStoreMoviesByGenresCommand.Execute(null);

                    BindingContext = vm;

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        MoviesList.ItemsSource = vm.AllMoviesList.Value;

                        await MoviesList.TranslateTo(0, 0, 500, Easing.SpringIn);

                        vm.ListVisible = true;

                        vm.IsEnabled = false;

                        vm.IsRunning = false;
                    });
                }
                else
                {

                    MoviesList.ItemsSource = vm.AllMoviesList.Value;

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        vm.ListVisible = true;
                        vm.IsRunning = false;
                        vm.IsEnabled = false;
                    });
                }
            }
            catch (Exception e2)
            {
                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("Error has ocurred!");
                    Debug.WriteLine("Error: " + e2.InnerException);
                    return false;
                });

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    vm.ListVisible = false;
                    vm.IsEnabled = false;
                    vm.IsRunning = false;
                    vm.MsgVisible = true;
                    vm.MsgText = "An unexpected error has ocurred, try again.";
                });

            }
        }

        private void RefreshBtnClicked(object sender, EventArgs e)
        {
            LoadMoreMovies().SafeFireAndForget();

            if (MainThread.IsMainThread)
            {
                scroll.ScrollToAsync(0, 500, true).SafeFireAndForget();

                RefreshBtn.TranslateTo(0, 80, 100, Easing.Linear).SafeFireAndForget();

                RefreshBtn.TranslateTo(0, 0, 100, Easing.Linear).SafeFireAndForget();
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(()=>
                {
                    scroll.ScrollToAsync(0, 500, true).SafeFireAndForget();

                    RefreshBtn.TranslateTo(0, 80, 100, Easing.Linear).SafeFireAndForget();

                    RefreshBtn.TranslateTo(0, 0, 100, Easing.Linear).SafeFireAndForget();
                });
            }
        }
    }
}