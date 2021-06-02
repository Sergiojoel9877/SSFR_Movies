using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Realms;
using Sharpnado.Tasks;
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
    public partial class AllMoviesPage : ContentPage
    {
        readonly AllMoviesPageViewModel vm = null;

        int DownCount { get; set; } = 1;

        ToolbarItem updownList = null;

        ToolbarItem searchToolbarItem = null;

        PullToRefreshLayout pull2refreshlyt;

        readonly MaterialSnackbarConfiguration _conf = new MaterialSnackbarConfiguration()
        {
            TintColor = Color.FromHex("#0066cc"),
            BackgroundColor = Color.FromHex("#272B2E")
        };

        public AllMoviesPage()
        {
            Settings.Down = false;

            InitializeComponent();

            vm = Locator.Current.GetService<AllMoviesPageViewModel>();

            BindingContext = vm;

            HideScrollAtStart();

            SetPullToRefresh();

            SetPull2RefreshToMainStack();

            //SuscribeToMessages();

            SetToolBarItems();

            SetScrollViewOrientation();

            ControlRaiseOverAllViews();

            SetListOrientationLayout();
        }

        private void SetListOrientationLayout()
        {
            MoviesList.ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
            {
                SnapPointsAlignment = SnapPointsAlignment.Start,
                SnapPointsType = SnapPointsType.None
            };
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
                Text = "Down",
                IconImageSource = "ListDown.png",
                Priority = 1,
                Command = new AsyncCommand(async ()=>
                {
                    var tapped = false;
                    if (updownList.Text == "Down" && tapped != true)
                    {
                        await Device.InvokeOnMainThreadAsync(async ()=>
                        {
                            await scrollview.TranslateTo(0, 20, 150, Easing.Linear);
                            tapped = true;
                            updownList.Text = "Up";
                        });
                    }
                    else
                    {
                        await Device.InvokeOnMainThreadAsync(async ()=>
                        {
                            await scrollview.TranslateTo(0, -80, 150, Easing.Linear);
                            tapped = false;
                            updownList.Text = "Down";
                        });
                    }
                })
            };

            searchToolbarItem = new ToolbarItem()
            {
                Text = "Search",
                IconImageSource = "Search.png",
                Priority = 0,
                Command = new AsyncCommand(async () =>
                {
                    await Shell.Current.GoToAsync("/Search", true);
                })
            };

            ToolbarItems.Add(updownList);

            ToolbarItems.Add(searchToolbarItem);
        }

        //private async Task<object> BringDownGenresBar()
        //{
        //    var tcs = new TaskCompletionSource<object>();

        //    await Device.InvokeOnMainThreadAsync(async () =>
        //    {
        //        await scrollview.TranslateTo(0, 80, 150, Easing.Linear);
        //        tcs.SetResult(null);
        //    });

        //    Settings.Down = false;
        //    return tcs.Task.Result;
        //}

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
            pull2refreshlyt.RefreshCommand = new AsyncCommand(()=>LoadMoreMovies());
        }

        private async void MovieSelected(object sender, SelectionChangedEventArgs e)
        {
            await ResultSingleton.SetInstanceAsync(((MoviesList.SelectedItem as ContentView).BindingContext as SwipeItem).BindingContext as Result);
            await Shell.Current.GoToAsync("/MovieDetails", true);
            MoviesList.SelectedItem = null;
        }

        /*private void SuscribeToMessages()
        {
            MessagingCenter.Subscribe<MovieDetailsPage>(this, "ClearSelection", (e) =>
            {
                MoviesList.SelectedItem = null;
            });
        }*/

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //SuscribeToMessages();

            Connectivity.ConnectivityChanged += Current_ConnectivityChanged;

            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    vm.NoNetWorkHideTabs.Value.ExecuteAsync().SafeFireAndForget();

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

                vm.GetStoreMoviesCommand.Value.ExecuteAsync().SafeFireAndForget();

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
                    vm.NoNetWorkHideTabs.Value.ExecuteAsync().SafeFireAndForget();
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
            try
            {
                await Task.Yield();
                
                //Verify if internet connection is available
                if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
                {
                    pull2refreshlyt.IsRefreshing = false;
                    await MaterialDialog.Instance.SnackbarAsync("No internet Connection", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf);
                    return;
                }

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    MoviesList.IsVisible = false;
                    //RefreshBtn.IsVisible = false;
                });

                Settings.NextPage++;

                vm.AllMoviesList.Value.Clear();

                var MoviesDownloaded = await Locator.Current.GetService<ApiClient>().GetAndStoreMoviesAsync(false, page: Settings.NextPage);

                if (MoviesDownloaded)
                {
                    BindingContext = null;

                    await vm.FillUpMoviesListAfterRefreshCommand.Value.ExecuteAsync();

                    if (vm.ListVisible)
                    {
                        BindingContext = vm;

                        await Device.InvokeOnMainThreadAsync(() =>
                        {
                            MoviesList.IsVisible = true;
                            activityIndicator.IsVisible = false;
                            //RefreshBtn.IsVisible = true;
                            pull2refreshlyt.IsRefreshing = false;
                        });
                    }
                }
            }
            catch (Exception e)
            {
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    pull2refreshlyt.IsRefreshing = false;
                    MoviesList.IsVisible = true;
                    activityIndicator.IsVisible = false;
                    //RefreshBtn.IsVisible = true;
                });

                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    //Task.Run(async () =>
                    //{
                    //    await MaterialDialog.Instance.SnackbarAsync("An error has ocurred!", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf);
                    //});
                    TaskMonitor.Create(MaterialDialog.Instance.SnackbarAsync("An error has ocurred!", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf));

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
                TaskMonitor.Create(()=> MaterialDialog.Instance.SnackbarAsync("No Internet connection", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf));
                return;
            }

            TaskMonitor.Create(Device.InvokeOnMainThreadAsync(() =>
            {
                vm.ListVisible = false;
                vm.IsEnabled = true;
                vm.IsRunning = true;
            }));

            MoviesList.ItemsSource = null;

            try
            {
                var realm = Realm.GetInstance();

                var genreType = ((Label)sender).Text;

                var genres = realm.All<Genres>().FirstOrDefault();

                var generId = genres.GenresGenres.Where(q => q.Name == genreType).FirstOrDefault().Id;

                var stored = await Locator.Current.GetService<ApiClient>().GetAndStoreMoviesByGenreAsync(generId, false);

                if (stored)
                {
                    await Device.InvokeOnMainThreadAsync(async ()=>
                    {
                        await MoviesList.TranslateTo(1500, 0, 500, Easing.SpringOut);
                    });

                    await vm.GetStoreMoviesByGenresCommand.Value.ExecuteAsync();

                    BindingContext = vm;

                    await Device.InvokeOnMainThreadAsync(async () =>
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

                    TaskMonitor.Create(Device.InvokeOnMainThreadAsync(() =>
                    {
                        vm.ListVisible = true;
                        vm.IsRunning = false;
                        vm.IsEnabled = false;
                    }));
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

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    vm.ListVisible = false;
                    vm.IsEnabled = false;
                    vm.IsRunning = false;
                    vm.MsgVisible = true;
                    vm.MsgText = "An unexpected error has ocurred, try again.";
                });
            }
        }

        //private void RefreshBtnClicked(object sender, EventArgs e)
        //{
        //    TaskMonitor.Create(LoadMoreMovies());

        //    TaskMonitor.Create(Device.InvokeOnMainThreadAsync(async () =>
        //    {
        //        await RefreshBtn.TranslateTo(0, 80, 100, Easing.Linear);

        //        await RefreshBtn.TranslateTo(0, 0, 100, Easing.Linear);
        //    }));
        //}

        private async void AddToFavListTap(object sender, EventArgs e)
        {
            await Task.Yield();
            //tap.Value.Tapped -= AddToFavListTap;

            //await pin2FavList.Value.ScaleTo(1.50, 500, Easing.BounceOut);

            if (sender != null)
            {
                var sndr = sender as SwipeItem;
                var movie = sndr.BindingContext as Result;

                //Verify if internet connection is available
                if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
                {
                    Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                    {
                        var conf = new MaterialSnackbarConfiguration()
                        {
                            TintColor = Color.FromHex("#0066cc"),
                            BackgroundColor = Color.FromHex("#272B2E")
                        };
                        MaterialDialog.Instance.SnackbarAsync("No internet Connection", "Dismiss", MaterialSnackbar.DurationIndefinite, conf);
                        return false;
                    });
                    return;
                }

                try
                {
                    var realm = await Realm.GetInstanceAsync();
                    var locker = new object();

                    var movieExists = default(Result);

                    lock (locker)
                    {
                        movieExists = realm.Find<Result>(movie.Id);
                    }

                    if (movieExists != null && movieExists.FavoriteMovie == "Star.png")
                    {
                        var _conf = new MaterialSnackbarConfiguration()
                        {
                            TintColor = Color.FromHex("#0066cc"),
                            BackgroundColor = Color.FromHex("#272B2E")
                        };

                        await MaterialDialog.Instance.SnackbarAsync("Oh no It looks like " + movie.Title + " already exits in your favorite list!", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf);

                        //await pin2FavList.Value.ScaleTo(1, 500, Easing.BounceIn);

                        return;
                    }

                    realm.Write(() =>
                    {
                        movie.FavoriteMovie = "Star.png";

                        realm.Add(movie, true);
                    });

                    var conf = new MaterialSnackbarConfiguration()
                    {
                        TintColor = Color.FromHex("#0066cc"),
                        BackgroundColor = Color.FromHex("#272B2E")
                    };

                    await MaterialDialog.Instance.SnackbarAsync("Added Successfully, The movie " + movie.Title + " was added to your favorite list!", "Dismiss", MaterialSnackbar.DurationShort, conf);

                    //await pin2FavList.Value.ScaleTo(1, 500, Easing.BounceIn);

                    MessagingCenter.Send(this, "Refresh", true);
                    //await Locator.Current.GetService<FavoriteMoviesPageViewModel>().FillMoviesList();
                }
                catch (Exception e15)
                {
                    Debug.WriteLine("Error: " + e15.InnerException);
                }
            }
        }

        void SwipeItem_Invoked(System.Object sender, System.EventArgs e)
        {
            TaskMonitor<object>.Create(ResultSingleton.SetInstanceAsync((sender as SwipeItem).BindingContext as Result)); 
            TaskMonitor.Create(Shell.Current.GoToAsync("/MovieDetails", true));
            MoviesList.SelectedItem = null;
        }
    }
}