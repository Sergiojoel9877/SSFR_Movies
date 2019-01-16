using System;
using System.Linq;
using System.Threading.Tasks;
using CommonServiceLocator;
using SSFR_Movies.Helpers;
using SSFR_Movies.Models;
using SSFR_Movies.Services;
using SSFR_Movies.ViewModels;
using MonkeyCache.FileStore;
using Plugin.Connectivity;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Xamarin.Forms.Internals;
using System.Threading;
using System.Diagnostics;
using Unity;
using Refractored.XamForms.PullToRefresh;

namespace SSFR_Movies.Views
{
    /// <summary>
    /// AllMoviesPage Code Behind
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllMoviesPage : ContentPage
    {
       
        AllMoviesPageViewModel vm = null;

        FlexLayout genresContainer = null;

        ToolbarItem updownList = null;

        ToolbarItem searchToolbarItem = null;

        PullToRefreshLayout pull2refreshlyt = null;
        
        public AllMoviesPage()
        {
            InitializeComponent();

            ContainerInitializer.Initialize();

            vm = ServiceLocator.Current.GetInstance<AllMoviesPageViewModel>();

            BindingContext = vm;

            pull2refreshlyt = new PullToRefreshLayout()
            {
                Content = scroll,
                RefreshBackgroundColor = Color.FromHex("#272B2E"),
                RefreshColor = Color.FromHex("#006FDE")
            };
            pull2refreshlyt.SetBinding(PullToRefreshLayout.IsRefreshingProperty, "IsRefreshing");
            pull2refreshlyt.SetBinding(PullToRefreshLayout.RefreshCommandProperty, "FillUpMoviesListAfterRefreshCommand");

            pull2refreshlyt.RefreshCommand = new Command(async () =>
            {
                await LoadMoreMovies();
            });

            stack.Children.Add(pull2refreshlyt);
            
            SuscribeToMessages();
            
            genresContainer = this.FindByName<FlexLayout>("GenresContainer");

            Device.BeginInvokeOnMainThread(async () =>
            {
                genresContainer.IsVisible = false;
                await Scrollview.TranslateTo(0, -80, 500, Easing.Linear);
            });

            searchToolbarItem = new ToolbarItem()
            {
                Text = "Search",
                Icon = "Search.png",
                Priority = 0,

                Command = new Command(async () =>
                {
                    await Navigation.PushAsync(new SearchPage(), false);
                })
            };

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
                        Device.BeginInvokeOnMainThread(async () => { 

                            genresContainer.IsVisible = false;

                            await Scrollview.TranslateTo(0, -80, 150, Easing.Linear);
                        });
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(async ()=>
                        {
                            genresContainer.IsVisible = true;

                            await Scrollview.TranslateTo(0, 0, 150, Easing.Linear);
                        });
                    }
                })
            };

            ToolbarItems.Add(searchToolbarItem);

            ToolbarItems.Add(updownList);
            
            Scrollview.Orientation = ScrollOrientation.Horizontal;
   
        }

        private void SuscribeToMessages()
        {
            MessagingCenter.Subscribe<CustomViewCell, bool>(this, "Hide", (s, e) =>
            {
                if (e == true)
                {
                    vm.MsgVisible = false;
                    MessageImg.Source = null;
                    MessageImg.TranslateTo(500, 0, 2);
                }
            });
        }

        private async void InitializeAsync(Func<Task> action)
        {
            await action();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
      
            CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;

            //Verify if internet connection is available
            if (!CrossConnectivity.Current.IsConnected)
            {
                vm.MsgVisible = true;
                vm.MsgText = "It seems like you don't have an internet connection!";
                vm.IsEnabled = false;
                vm.IsRunning = false;

                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");
                    return false;
                });
                return;
            }
            else
            {
                vm.MsgVisible = false;
            }
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

            vm.MsgVisible = false;

            CrossConnectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;
        }

        private void Current_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        {
            if(e.IsConnected)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    vm.MsgVisible = false;
                    vm.ListVisible = true;
                    MessageImg.Source = ImageSource.FromFile("NoInternet.png");
                    MessageImg.TranslateTo(0, 0, 2);
                });

                vm.GetStoreMoviesCommand.Execute(null);
          
                BindingContext = vm;

                Device.BeginInvokeOnMainThread(()=>
                {
                    MoviesList.ItemsSource = null;
                    MoviesList.ItemsSource = vm.AllMoviesList;
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    vm.MsgVisible = true;
                    vm.MsgText = "It seems like you don't have an internet connection!";
                    vm.ListVisible = false;
                    vm.IsRunning = false;
                    vm.IsEnabled = false;
                    DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");
                });
            }
        }

        private async void MoviesList_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            await Task.Yield();

            try
            {
                var list = (ListView)sender;

                var Items = vm.AllMoviesList;

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
                                        
                    await LoadMoreMovies();
                }
            }
            catch (Exception e1)
            {

                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("An error has occurred!" + e1.InnerException);

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

                Settings.NextPage++;

                vm.AllMoviesList.Clear();
                
                var token = new CancellationTokenSource();

                token.CancelAfter(4000);

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
                            pull2refreshlyt.IsRefreshing = false;
                        });
                    }
                }
            }
            catch (Exception e)
            {
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

            Device.BeginInvokeOnMainThread(() =>
            {
                vm.ListVisible = false;
                vm.IsEnabled = true;
                vm.IsRunning = true;
            });

            MoviesList.ItemsSource = null;

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
                    
                    BindingContext = vm;

                    Device.BeginInvokeOnMainThread(async () =>
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

                Device.BeginInvokeOnMainThread(()=>
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
            InitializeAsync( async () =>
            {
                await Navigation.PushAsync(new SearchPage(), false);
            });
           
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (Parent == null)
            {
                BindingContext = null;
            }
        }

        private void RefreshBtnClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(()=>
            {
                activityIndicator.IsRunning = true;
                activityIndicator.IsVisible = true;
                RefreshBtn.IsEnabled = false;
                pull2refreshlyt.IsPullToRefreshEnabled = false;
            });

            InitializeAsync(async () =>
            {
                await LoadMoreMovies();
                
                await scroll.ScrollToAsync(0, 500, true);

                await RefreshBtn.TranslateTo(0, 80, 100, Easing.Linear);

                await RefreshBtn.TranslateTo(0, 0, 100, Easing.Linear);
            });

            Device.BeginInvokeOnMainThread(()=>
            {
                activityIndicator.IsRunning = false;
                activityIndicator.IsVisible = false;
                RefreshBtn.IsEnabled = true;
                pull2refreshlyt.IsPullToRefreshEnabled = true;
            });
        }
    }
}