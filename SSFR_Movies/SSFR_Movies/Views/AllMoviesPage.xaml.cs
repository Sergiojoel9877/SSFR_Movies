using AsyncAwaitBestPractices;
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

            HideScrollAtStart();
         
            vm = Locator.Current.GetService<AllMoviesPageViewModel>();

            BindingContext = vm;
            
            SetPullToRefresh();

            SetPull2RefreshToMainStack();
            
            SuscribeToMessages();

            SetToolBarItems();

            SetScrollViewOrientation();

            ControlRaiseOverAllViews();
        }

        private void HideScrollAtStart()
        {
            scrollview.TranslationY = -80;
        }

        private void ControlRaiseOverAllViews()
        {
            stack.RaiseChild(scrollview);
        }

        private void SetScrollViewOrientation()
        {
            scrollview.Orientation = ScrollOrientation.Horizontal;
        }

        private void SetToolBarItems()
        {
            updownList = new ToolbarItem()
            {
                Text = "Up",
                IconImageSource = "ListDown.png",
                Priority = 1,
                Command = new Command(() =>
                {
                    //Settings.Down = false;

                    if (Settings.Down)
                    {
                        Settings.Down = false;
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await scrollview.TranslateTo(0, -80, 150, Easing.Linear);
                        });
                    }
                    else
                    {
                        Settings.Down = true;
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await scrollview.TranslateTo(0, 0, 150, Easing.Linear);
                        });
                    }
                })
            };

            searchToolbarItem = new ToolbarItem()
            {
                Text = "Search",
                IconImageSource = "Search.png",
                Priority = 0,

                Command = new Command(() =>
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Shell.Current.GoToAsync("/Search", true);
                    });
                })
            };

            ToolbarItems.Add(updownList);

            ToolbarItems.Add(searchToolbarItem);

        }

        private void SetPull2RefreshToMainStack()
        {
            AbsoluteLayout.SetLayoutBounds(pull2refreshlyt, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(pull2refreshlyt, AbsoluteLayoutFlags.All);

            stack.Children.Add(pull2refreshlyt);
        }

        private void SetPullToRefresh()
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
        }

        private async void MovieSelected(object sender, SelectionChangedEventArgs e)
        {
            if (MoviesList.SelectedItem != null)
            {
                var movie = MoviesList.SelectedItem as Result;

                ResultSingleton.SetInstance(movie);

                //Device.BeginInvokeOnMainThread(async ()=>
                //{
                await Shell.Current.GoToAsync("/MovieDetails", false);
                //});
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
                Device.BeginInvokeOnMainThread(() =>
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

                Device.BeginInvokeOnMainThread(() =>
                {
                    vm.ListVisible = true;
                });

                vm.GetStoreMoviesCommand.Execute(null);

                BindingContext = vm;

                Device.BeginInvokeOnMainThread(() =>
                {
                    MoviesList.ItemsSource = null;
                    MoviesList.ItemsSource = vm.AllMoviesList.Value;
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
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
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        MoviesList.IsVisible = false;
                        RefreshBtn.IsVisible = false;
                    });

                Settings.NextPage++;

                vm.AllMoviesList.Value.Clear();

                var MoviesDownloaded = await Locator.Current.GetService<ApiClient>().GetAndStoreMoviesAsync(false, page: Settings.NextPage);

                if (MoviesDownloaded)
                {
                    BindingContext = null;

                    vm.FillUpMoviesListAfterRefreshCommand.Execute(null);

                    if (vm.ListVisible)
                    {
                        BindingContext = vm;

                            Device.BeginInvokeOnMainThread(() =>
                            {
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
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        pull2refreshlyt.IsRefreshing = false;
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
                Device.BeginInvokeOnMainThread(() =>
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

                        Device.BeginInvokeOnMainThread(async () =>
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

                    Device.BeginInvokeOnMainThread(() =>
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

                Device.BeginInvokeOnMainThread(() =>
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

                Device.BeginInvokeOnMainThread(()=>
                {
                    RefreshBtn.TranslateTo(0, 80, 100, Easing.Linear).SafeFireAndForget();

                    RefreshBtn.TranslateTo(0, 0, 100, Easing.Linear).SafeFireAndForget();
                });
        }

    }
}