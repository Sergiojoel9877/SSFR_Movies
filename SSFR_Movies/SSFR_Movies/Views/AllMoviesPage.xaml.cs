﻿using System;
using System.Linq;
using System.Threading.Tasks;
using CommonServiceLocator;
using SSFR_Movies.Helpers;
using SSFR_Movies.Models;
using SSFR_Movies.Services;
using SSFR_Movies.ViewModels;
using MonkeyCache.FileStore;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Xamarin.Forms.Internals;
using System.Threading;
using System.Diagnostics;

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
        
        public AllMoviesPage()
        {
            InitializeComponent();
            
            vm = ServiceLocator.Current.GetInstance<AllMoviesPageViewModel>();

            BindingContext = vm;

            SuscribeToMessages();
            
            //Message.IsVisible = false;

            //MessageImg.IsVisible = false;

            genresContainer = this.FindByName<FlexLayout>("GenresContainer");

            genresContainer.IsVisible = false;

            Task.Factory.StartNew(async () => { await Scrollview.TranslateTo(0, -60, 500, Easing.Linear); });

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

                Command = new Command(async () =>
                {
                    updownList.Icon = updownList.Icon == "ListDown.png" ? "ListUp.png" : "ListDown.png";

                    if (updownList.Icon == "ListDown.png")
                    {
                        genresContainer.IsVisible = false;

                        var t = Scrollview.TranslateTo(0, -60, 150, Easing.Linear);

                        await Task.WhenAll(t);
                    }
                    else
                    {

                        genresContainer.IsVisible = true;

                        var t = Scrollview.TranslateTo(0, 0, 150, Easing.Linear);

                        await Task.WhenAll(t);

                    }
                })
            };

            ToolbarItems.Add(searchToolbarItem);

            ToolbarItems.Add(updownList);
            
            Scrollview.Orientation = ScrollOrientation.Horizontal;

            CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
            
            //Task.Run(async ()=>
            //{
            //    await InitializeMsg(async () =>
            //    {
            //        await SpeakNow("Initializing resources, please wait a sencond.");
            //    });
            //});
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
                    //Message.IsVisible = false;
                }
            });
        }

        private async void InitializeMsg(Func<Task> action)
        {
            await action();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
      
            var t5 = Scrollview.ScrollToAsync(100, 0, true);

            var t6 = Scrollview.ScrollToAsync(0, 0, true);

            await Task.WhenAll(t5, t6);

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
                    //MoviesList.BeginRefresh();
                });

                vm.GetStoreMoviesCommand.Execute(null);
          
                BindingContext = vm;

                Device.BeginInvokeOnMainThread(()=>
                {
                    //MoviesList.EndRefresh();
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

                    Device.BeginInvokeOnMainThread(()=>
                    {
                        //MoviesList.BeginRefresh();
                    });

                    //Parallel.Invoke(async ()=>
                    //{
                        await LoadMoreMovies();
                    //});
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

                vm.AllMoviesList.Clear();
                
                var token = new CancellationTokenSource();

                token.CancelAfter(4000);

                var MoviesDownloaded = await ServiceLocator.Current.GetInstance<ApiClient>().GetAndStoreMoviesAsync(false, token, page: Settings.NextPage++);

                if (MoviesDownloaded)
                {
                    BindingContext = null;

                    vm.FillUpMoviesListAfterRefreshCommand.Execute(null);

                    if (vm.ListVisible)
                    {
                        BindingContext = vm;

                        Device.BeginInvokeOnMainThread( () =>
                        {
                            //MoviesList.EndRefresh();
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
                //MoviesList.BeginRefresh();
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

                        //MoviesList.EndRefresh();

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
                        //MoviesList.EndRefresh();

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

                //MoviesList.EndRefresh();
                
            }
        }

        private async void SearchClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SearchPage(), false);
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (Parent == null)
            {
                BindingContext = null;
            }
        }
    }
}