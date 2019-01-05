using SSFR_Movies.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MonkeyCache.FileStore;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using System.Linq;
using System.Threading.Tasks;
using CommonServiceLocator;
using SSFR_Movies.Services;
using SSFR_Movies.Helpers;
using Plugin.Connectivity;
using System.Threading;
using Xamarin.Essentials;

namespace SSFR_Movies.ViewModels
{
    /// <summary>
    /// AllMoviesPage View Model
    /// </summary>
    [Preserve(AllMembers = true)]
    public class AllMoviesPageViewModel : ViewModelBase
    {
        public ObservableCollection<Result> AllMoviesList { get; set; } = new ObservableCollection<Result>();

        private bool listVisible = false;
        public bool ListVisible
        {
            get => listVisible;
            set => SetProperty(ref listVisible, value);
        }

        private bool msgVisible = false;
        public bool MsgVisible
        {
            get => msgVisible;
            set => SetProperty(ref msgVisible, value);
        }

        private string msgText;
        public string MsgText
        {
            get => msgText;
            set => SetProperty(ref msgText, value);
        }

        private bool isRefreshing;
        public bool IsRefreshing
        {
            get => isRefreshing;
            set => SetProperty(ref isRefreshing, value);
        }

        private bool isEnabled = false;
        public bool IsEnabled
        {
            get => isEnabled;
            set => SetProperty(ref isEnabled, value);
        }

        private bool animationEnabled = false;
        public bool AnimationEnabled
        {
            get => animationEnabled;
            set => SetProperty(ref animationEnabled, value);
        }

        private bool isRunning;
        public bool IsRunning
        {
            get => isRunning;
            set => SetProperty(ref isRunning, value);
        }

        private bool moviesStored = false;
        public bool MoviesStored
        {
            get => moviesStored;
            set => SetProperty(ref moviesStored, value);
        }

        private bool activityIndicatorRunning = true;
        public bool ActivityIndicatorRunning
        {
            get => activityIndicatorRunning;
            set => SetProperty(ref activityIndicatorRunning, value);
        }

        public async Task FillMoviesList()
        {
            await Task.Yield();

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

            Device.BeginInvokeOnMainThread(() =>
            {
                IsEnabled = true;
                IsRunning = true;
            });

            var movies = Barrel.Current.Get<Movie>("Movies.Cached");

            if (movies == null)
            {
                return;
            }

            movies.Results.ForEach((r)=>
            {
                AllMoviesList.Add(r);
            });

            Device.BeginInvokeOnMainThread(()=>
            {
                ListVisible = true;
                MsgVisible = false;
                IsRefreshing = false;
                IsEnabled = false;
                IsRunning = false;
            });
        }

        public async Task FillMoviesByGenreList()
        {
            await Task.Yield();
    
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

            var movies = Barrel.Current.Get<Movie>("MoviesByXGenre.Cached");

            AllMoviesList.Clear();

            movies.Results.ForEach((r)=>
            {
                AllMoviesList.Add(r);
            });

            Device.BeginInvokeOnMainThread(()=>
            {
                IsRunning = false;
                IsEnabled = false;
            });
 
        }

        /// <summary>
        ///  Get movies from server and store them in cache.. with a TimeSpan limit.
        /// </summary>
        /// <returns>Bool if they are succesfully saved..</returns>
        private static async Task<bool> GetAndStoreMoviesAsync()
        {
            
            //Verify if internet connection is available
            if (!CrossConnectivity.Current.IsConnected)
            {
                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");
                    return false;
                });
                return false;
            }

            var token = new CancellationTokenSource();

            token.CancelAfter(4000);

            var done = await ServiceLocator.Current.GetInstance<ApiClient>().GetAndStoreMoviesAsync(false, token);

            if (done)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Command getStoreMoviesCommand;
        public Command GetStoreMoviesCommand
        {
            get => getStoreMoviesCommand ?? (getStoreMoviesCommand = new Command( async () =>
            {
                await Task.Yield();

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

                Device.BeginInvokeOnMainThread(() =>
                {
                    IsRunning = true;
                    IsEnabled = true;
                });

                var stored = await GetAndStoreMoviesAsync();

                if (!stored) 
                {
                    Device.BeginInvokeOnMainThread(()=>
                    {
                        MsgVisible = true;
                        MsgText = "Low storage left!";
                        IsRunning = false;
                        IsEnabled = false;
                    });
                }

                MoviesStored = stored;

                if (MoviesStored)
                {
                    await FillMoviesList();
                }

            }));
        }

        private Command getStoreMoviesByGenresCommand;
        public Command GetStoreMoviesByGenresCommand
        {
            get => getStoreMoviesByGenresCommand ?? (getStoreMoviesByGenresCommand = new Command(async () =>
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

                await FillMoviesByGenreList();
         
            }));
        }
        
        private Command getMoviesGenresCommand;
        public Command GetMoviesGenresCommand
        {
            get => getMoviesGenresCommand ?? (getMoviesGenresCommand = new Command(async () =>
            {
                await Task.Yield();
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

                var done = await GetMoviesGenres();

                if (!done)
                {
                    Device.BeginInvokeOnMainThread(()=>
                    {
                        MsgVisible = true;
                        MsgText = "No storage space left!";
                    });
                }
            }));
        }

        private async Task<bool> GetMoviesGenres()
        {
            await Task.Yield();

            //Verify if internet connection is available
            if (!CrossConnectivity.Current.IsConnected)
            {
                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");
                    return false;
                });
                return false;
            }
            
            //return ServiceLocator.Current.GetInstance<ApiClient>().GetAndStoreMovieGenresAsync();
            return await ServiceLocator.Current.GetInstance<ApiClient>().GetAndStoreMovieGenresAsync();

        }

        private Command fillUpMoviesListAfterRefreshCommand;
        public Command FillUpMoviesListAfterRefreshCommand
        {
            get => fillUpMoviesListAfterRefreshCommand ?? (fillUpMoviesListAfterRefreshCommand = new Command( async () =>
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
                
                await FillMoviesList();

            }));
        }

        public AllMoviesPageViewModel()
        {
            Device.BeginInvokeOnMainThread(()=>
            {
                MsgVisible = false;
                IsRunning = true;
                IsEnabled = true;
            });

            //CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
            
            //Verify if internet connection is available
            if (!CrossConnectivity.Current.IsConnected)
            {
                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("Please be sure that your device has  an Internet connection");
                    return false;
                });

                Device.BeginInvokeOnMainThread(() =>
                {
                    MsgVisible = true;
                });

                return;
            }

            //If the barrel cache doesn't exits or its expired.. Get the movies again and store them..
            if (!Barrel.Current.Exists("Movies.Cached") || Barrel.Current.IsExpired("Movies.Cached"))
            {
                //Parallel.Invoke(()=>
                //{
                //    GetStoreMoviesCommand.Execute(null);
                //});

                //Parallel.Invoke(() =>
                //{
                //    GetMoviesGenresCommand.Execute(null);
                //});

                GetStoreMoviesCommand.Execute(null);

                GetMoviesGenresCommand.Execute(null);
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ListVisible = false;

                    MsgVisible = false;

                    ActivityIndicatorRunning = true;
                });
               
                Task.Factory.StartNew(async () => { await FillMoviesList(); });
            }
        }

        //private void Current_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        //{
        //    Device.BeginInvokeOnMainThread(() =>
        //    {
        //        if (e.IsConnected)
        //        {
        //            MsgVisible = false;
        //        }
        //        else
        //        {
        //            MsgVisible = true;
        //        }
        //    });

        //    GetStoreMoviesCommand.Execute(null);

        //    GetMoviesGenresCommand.Execute(null);
        //}
    }
}
