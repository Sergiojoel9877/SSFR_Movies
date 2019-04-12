using System;
using System.Linq;
using System.Threading.Tasks;
using SSFR_Movies.Helpers;
using SSFR_Movies.Models;
using SSFR_Movies.Services;
using SSFR_Movies.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Xamarin.Forms.Internals;
using System.Threading;
using System.Diagnostics;
using Refractored.XamForms.PullToRefresh;
using static SSFR_Movies.Views.SearchPage;
using Realms;
using Splat;
using XF.Material.Forms.UI.Dialogs;

namespace SSFR_Movies.Views
{
    /// <summary>
    /// AllMoviesPage Code Behind
    /// </summary>
    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllMoviesPage : ContentPage
    {
       
        AllMoviesPageViewModel vm = null;

        FlexLayout genresContainer = null;

        ToolbarItem updownList = null;

        ToolbarItem searchToolbarItem = null;

        readonly PullToRefreshLayout pull2refreshlyt = new PullToRefreshLayout();
        
        public AllMoviesPage()
        {
            InitializeComponent();

            vm = Locator.CurrentMutable.GetService<AllMoviesPageViewModel>();

            BindingContext = vm;

            pull2refreshlyt = new PullToRefreshLayout()
            {
                Content = scroll,
                RefreshBackgroundColor = Color.FromHex("#272B2E"),
                RefreshColor = Color.FromHex("#006FDE")
            };

            var swipeGesture = new SwipeGestureRecognizer
            {
                Direction = SwipeDirection.Left
            };
            swipeGesture.Swiped += SwipeGestureRecognizer_Swiped;
            Content.Content.GestureRecognizers.Add(swipeGesture);
           
            pull2refreshlyt.RefreshCommand = new Command(async () =>
            {
                await LoadMoreMovies();
            });

            stack.Children.Add(pull2refreshlyt);
            
            SuscribeToMessages();
            
            genresContainer = this.FindByName<FlexLayout>("GenresContainer");

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                genresContainer.IsVisible = false;
                await Scrollview.TranslateTo(0, -80, 500, Easing.Linear);
            });

            MoviesList.SelectionChangedCommand = new Command(MovieSelected);
            
            updownList = new ToolbarItem()
            {
                Text = "Up",
                Icon = "ListDown.png",
                Priority = 1,
                Command = new Command(() =>
                {
                    updownList.Icon = updownList.Icon == "ListDown.png" ? "ListUp.png" : "ListDown.png";

                    if (updownList.Icon == "ListDown.png")
                    {
                        MainThread.BeginInvokeOnMainThread(async () => { 

                            genresContainer.IsVisible = false;

                            await Scrollview.TranslateTo(0, -80, 150, Easing.Linear);
                        });
                    }
                    else
                    {
                        MainThread.BeginInvokeOnMainThread(async ()=>
                        {
                            genresContainer.IsVisible = true;

                            await Scrollview.TranslateTo(0, 0, 150, Easing.Linear);
                        });
                    }
                })
            };
            
            searchToolbarItem = new ToolbarItem()
            {
                Text = "Search",
                Icon = "Search.png",
                Priority = 0,

                Command = new Command(() =>
                {
                    MainThread.BeginInvokeOnMainThread(async ()=>
                    {
                        await Navigation.PushAsync(new SearchPage(), true);
                    });
                })
            };

            MoviesList.SelectionChangedCommand = new Command(MovieSelected);
            
            ToolbarItems.Add(updownList);

            ToolbarItems.Add(searchToolbarItem);
            
            Scrollview.Orientation = ScrollOrientation.Horizontal;
        }

        private void AllMoviesPage_Swiped(object sender, SwipedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MovieSelected()
        {
            if (MoviesList.SelectedItem != null)
            {
                var movie = MoviesList.SelectedItem as Result;

                MainThread.BeginInvokeOnMainThread(async ()=> 
                {
                    await Navigation.PushAsync(new MovieDetailsPage(movie));
                });
            }
        }
        
        private void SuscribeToMessages()
        {
            MessagingCenter.Subscribe<CustomViewCell, bool>(this, "Hide", async (s, e) =>
            {
                if (e == true)
                {
                    vm.MsgVisible = false;
                    MessageImg.Source = null;
                    await MessageImg.TranslateTo(500, 0, 2);
                }
            });

            MessagingCenter.Subscribe<MovieDetailsPage>(this, "ClearSelection", (e) =>
            {
                MoviesList.SelectedItem = null;
            });

            //MessagingCenter.Subscribe<AllMoviesPageViewModel>(this, "HIDE", (p)=>
            //{
            //    Shell.SetTabBarIsVisible(this, false);
            //});
        }

        private async void InitializeAsync(Func<Task> action)
        {
            await action();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    vm.NoNetWorkHideTabs.Execute(null);
                    Shell.SetTitleView(this, null);
                    Shell.SetTitleView(this, new Label() { Text = "Connecting...", TextColor = Color.White, FontAttributes = FontAttributes.Bold, FontSize = 20, VerticalTextAlignment = TextAlignment.Center });
                    Shell.SetTabBarIsVisible(this, false);
                });
                return;
            }

            SuscribeToMessages();
            
            Connectivity.ConnectivityChanged += Current_ConnectivityChanged;
        }

        private async Task SpeakNow(string msg)
        {
            var settings = new SpeechOptions()
            {
                Pitch = 1f,
                Volume = 1f
            };

           await TextToSpeech.SpeakAsync(msg, settings);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            Connectivity.ConnectivityChanged += Current_ConnectivityChanged;
        }

        private void Current_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if(e.NetworkAccess == NetworkAccess.Internet)
            {
                Shell.SetTitleView(this, null);
                Shell.SetTitleView(this, new Label() { Text = "SSFR Movies", TextColor = Color.White, FontAttributes = FontAttributes.Bold, FontSize = 20, VerticalTextAlignment = TextAlignment.Center });
                Shell.SetTabBarIsVisible(this, true);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    vm.MsgVisible = false;
                    vm.ListVisible = true;
                    MessageImg.TranslateTo(0, 0, 2);
                });

                vm.GetStoreMoviesCommand.Execute(null);
          
                BindingContext = vm;

                MainThread.BeginInvokeOnMainThread(()=>
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
                    Shell.SetTitleView(this, new Label() { Text = "Connecting...", TextColor = Color.White, FontAttributes = FontAttributes.Bold, FontSize = 20, VerticalTextAlignment = TextAlignment.Center });
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
                    //Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                    //{
                    //    DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");
                    //    return false;
                    //});
                    pull2refreshlyt.IsRefreshing = false;
                    await MaterialDialog.Instance.SnackbarAsync("No internet Connection", "Dismiss", MaterialSnackbar.DurationIndefinite);
                    return;
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    //activityIndicator.IsRunning = true;
                    //activityIndicator.IsVisible = true;
                    MoviesList.IsVisible = false;
                    RefreshBtn.IsEnabled = false;
                });

                Settings.NextPage++;

                vm.AllMoviesList.Value.Clear();
                
                var token = new CancellationTokenSource();

                token.CancelAfter(4000);

                //var MoviesDownloaded = await ServiceLocator.Current.GetInstance<Lazy<ApiClient>>().Value.GetAndStoreMoviesAsync(false, page: Settings.NextPage);
                var MoviesDownloaded = await Locator.CurrentMutable.GetService<ApiClient>().GetAndStoreMoviesAsync(false, page: Settings.NextPage);
                
                if (MoviesDownloaded)
                {
                    BindingContext = null;

                    vm.FillUpMoviesListAfterRefreshCommand.Execute(null);

                    if (vm.ListVisible)
                    {
                        BindingContext = vm;

                        MainThread.BeginInvokeOnMainThread( () =>
                        {
                            //pull2refreshlyt.IsRefreshing = false;
                            //activityIndicator.IsRunning = false;
                            MoviesList.IsVisible = true;
                            activityIndicator.IsVisible = false;
                            RefreshBtn.IsEnabled = true;
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
                    RefreshBtn.IsEnabled = true;
                });

                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("An error has ocurred!");
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
                //Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                //{
                //    DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");
                //    return false;
                //});
                 await MaterialDialog.Instance.SnackbarAsync("No internet Connection", "Dismiss", MaterialSnackbar.DurationIndefinite);

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
                var stored = await Locator.CurrentMutable.GetService<ApiClient>().GetAndStoreMoviesByGenreAsync((int)generId, false);
    
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

                MainThread.BeginInvokeOnMainThread(()=>
                {
                    vm.ListVisible = false;
                    vm.IsEnabled = false;
                    vm.IsRunning = false;
                    vm.MsgVisible = true;
                    vm.MsgText = "An unexpected error has ocurred, try again.";
                });  
                
            }
        }

        private void SearchClicked(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async ()=>
            {
                await Navigation.PushAsync(new SearchPage(), true);
            });
           
        }
        
        private void RefreshBtnClicked(object sender, EventArgs e)
        {

            InitializeAsync(async () =>
            {
                await LoadMoreMovies();
                
                await scroll.ScrollToAsync(0, 500, true);

                await RefreshBtn.TranslateTo(0, 80, 100, Easing.Linear);

                await RefreshBtn.TranslateTo(0, 0, 100, Easing.Linear);
            });
            
        }

        private async void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {
            await Navigation.PushAsync(new FavoritesMoviesPage(), true);
        }
    }
}