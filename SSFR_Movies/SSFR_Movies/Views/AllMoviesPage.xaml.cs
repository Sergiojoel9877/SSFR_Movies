using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Realms;
using Splat;
using SSFR_Movies.Helpers;
using SSFR_Movies.Models;
using SSFR_Movies.Services;
using SSFR_Movies.Services.Abstract;
using SSFR_Movies.ViewModels;
using SSFR_Movies.Views.DataTemplateSelectors;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.CommunityToolkit.ObjectModel;
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

        RefreshView pull2refreshlyt;

        readonly MaterialSnackbarConfiguration _conf = new()
        {
            TintColor = Color.FromHex("#0066cc"),
            BackgroundColor = Color.FromHex("#272B2E")
        };

        public AllMoviesPage()
        {
            Settings.Down = false;

            InitializeComponent();

            vm = Locator.Current.GetService<AllMoviesPageViewModel>();

            SetCollectionViewItemTemplate();

            BindingContext = vm;

            HideScrollAtStart();

            SetPullToRefresh();

            SetPull2RefreshToMainStack();

            SetToolBarItems();

            SetScrollViewOrientation();

            ControlRaiseOverAllViews();

            SetListOrientationLayout();
        }

        void SetCollectionViewItemTemplate()
        {
            MoviesList.ItemTemplate = new SelectedMovieTemplateSelector();
            MoviesList.Bind(CollectionView.ItemsSourceProperty, nameof(AllMoviesPageViewModel.AllMoviesList));
        }

        private void SetListOrientationLayout()
        {
            MoviesList.ItemsLayout = new GridItemsLayout(2, ItemsLayoutOrientation.Vertical);
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
                        await MainThread.InvokeOnMainThreadAsync(async ()=>
                        {
                            await scrollview.TranslateTo(0, 20, 150, Easing.Linear);
                            tapped = true;
                            updownList.Text = "Up";
                        });
                    }
                    else
                    {
                        await MainThread.InvokeOnMainThreadAsync(async ()=>
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

        private async Task<object> BringDownGenresBar()
        {
            var tcs = new TaskCompletionSource<object>();

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await scrollview.TranslateTo(0, 80, 150, Easing.Linear);
                tcs.SetResult(null);
            });

            Settings.Down = false;
            return tcs.Task.Result;
        }

        private void SetPull2RefreshToMainStack()
        {
            AbsoluteLayout.SetLayoutBounds(pull2refreshlyt, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(pull2refreshlyt, AbsoluteLayoutFlags.All);

            stack.Children.Add(pull2refreshlyt);
        }

        private void SetPullToRefresh()
        {
            pull2refreshlyt = new(); 
            pull2refreshlyt.Content = scroll;
            pull2refreshlyt.RefreshColor = Color.FromHex("#272B2E");
            pull2refreshlyt.RefreshColor = Color.FromHex("#006FDE");
            pull2refreshlyt.SetBinding(RefreshView.CommandProperty, "FillUpMoviesListAfterRefreshCommand");
            pull2refreshlyt.Command = new AsyncCommand(()=>LoadMoreMovies());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Connectivity.ConnectivityChanged += Current_ConnectivityChanged;

            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                MainThread.BeginInvokeOnMainThread(() =>
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

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    vm.ListVisible = true;
                });

                vm.GetStoreMoviesCommand.Value.ExecuteAsync().SafeFireAndForget();

                BindingContext = vm;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MoviesList.ItemsSource = null;
                    MoviesList.ItemsSource = vm.AllMoviesList;
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
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

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    MoviesList.IsVisible = false;
                });

                Settings.NextPage++;

                vm.AllMoviesList.Clear();

                var MoviesDownloaded = await Locator.Current.GetService<ApiClient>().GetAndStoreMoviesAsync(false, page: Settings.NextPage);

                if (MoviesDownloaded)
                {
                    BindingContext = null;

                    await vm.FillUpMoviesListAfterRefreshCommand.Value.ExecuteAsync();

                    if (vm.ListVisible)
                    {
                        BindingContext = vm;

                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            MoviesList.IsVisible = true;
                            activityIndicator.IsVisible = false;
                            pull2refreshlyt.IsRefreshing = false;
                        });
                    }
                }
            }
            catch (Exception e)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    pull2refreshlyt.IsRefreshing = false;
                    MoviesList.IsVisible = true;
                    activityIndicator.IsVisible = false;
                });

                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    Task.Run(async ()=> await MaterialDialog.Instance.SnackbarAsync("An error has ocurred!", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf));

                    Debug.WriteLine("Error: " + e.InnerException);
                    return false;
                });
            }
        }

        private async void Genre_Tapped(object sender, EventArgs e)
        {
            //Verify if internet connection is available
            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                await MaterialDialog.Instance.SnackbarAsync("No Internet connection", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf);
                return;
            }

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                vm.ListVisible = false;
                vm.IsEnabled = true;
                vm.IsRunning = true;
            });

            MoviesList.ItemsSource = null;

            try
            {
                var realm = RealmDBSingleton.Current;

                var genreType = ((Label)sender).Text;

                var genres = realm.All<Genres>().FirstOrDefault();

                var generId = genres.GenresGenres.Where(q => q.Name == genreType).FirstOrDefault().Id;

                var stored = await Locator.Current.GetService<ApiClient>().GetAndStoreMoviesByGenreAsync(generId, false);

                if (stored)
                {
                    await MainThread.InvokeOnMainThreadAsync(async ()=>
                    {
                        await MoviesList.TranslateTo(1500, 0, 500, Easing.SpringOut);
                    });

                    await vm.GetStoreMoviesByGenresCommand.Value.ExecuteAsync();

                    BindingContext = vm;

                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        MoviesList.ItemsSource = vm.AllMoviesList;

                        await MoviesList.TranslateTo(0, 0, 500, Easing.SpringIn);

                        vm.ListVisible = true;

                        vm.IsEnabled = false;

                        vm.IsRunning = false;
                    });
                }
                else
                {
                    MoviesList.ItemsSource = vm.AllMoviesList;

                    await MainThread.InvokeOnMainThreadAsync(() =>
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

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    vm.ListVisible = false;
                    vm.IsEnabled = false;
                    vm.IsRunning = false;
                    vm.MsgVisible = true;
                    vm.MsgText = "An unexpected error has ocurred, try again.";
                });
            }
        }
    }
}