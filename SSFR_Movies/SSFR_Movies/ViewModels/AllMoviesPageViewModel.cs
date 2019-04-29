using SSFR_Movies.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using System.Linq;
using System.Threading.Tasks;
using Splat;
using SSFR_Movies.Services;
using SSFR_Movies.Helpers;
using System.Threading;
using Xamarin.Essentials;
using Realms;
//using ReactiveUI.Legacy;
using XF.Material.Forms.UI.Dialogs;
using XF.Material.Forms.UI.Dialogs.Configurations;
using SSFR_Movies.Views;

namespace SSFR_Movies.ViewModels
{
    /// <summary>
    /// AllMoviesPage View Model
    /// </summary>
    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    public class AllMoviesPageViewModel : ViewModelBase
    {
        public Lazy<ObservableCollection<Result>> AllMoviesList { get; set; } = new Lazy<ObservableCollection<Result>>(() => new ObservableCollection<Result>());

        readonly MaterialSnackbarConfiguration _conf = new MaterialSnackbarConfiguration()
        {
            TintColor = Color.FromHex("#0066cc"),
            BackgroundColor = Color.FromHex("#272B2E")
        };

        private bool listVisible = true;
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
        
        private bool isMLEnabled = true;
        public bool IsMLEnabled
        {
            get => isMLEnabled;
            set => SetProperty(ref isMLEnabled, value);
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
            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                await MaterialDialog.Instance.SnackbarAsync("No internet Connection", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf);

                return;
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                ListVisible = false;
                IsEnabled = true;
                IsRunning = true;
            });

            var realm = await Realm.GetInstanceAsync();

            var movies = realm.All<Movie>().SingleOrDefault();

            if (movies == null)
            {
                return;
            }

            movies.Results.ForEach((r)=>
            {
                AllMoviesList.Value.Add(r);
            });

            Device.BeginInvokeOnMainThread(() =>
            {
                ListVisible = true;
                MsgVisible = false;
                IsEnabled = false;
                IsRunning = false;
            });
        }

        public void FillMoviesByGenreList()
        {
                
            //Verify if internet connection is available
            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                MainThread.BeginInvokeOnMainThread(async ()=>
                {
                    await MaterialDialog.Instance.SnackbarAsync("No internet Connection", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf);
                });
               
                return;
            }
            var realm = Realm.GetInstance();

            var movies = realm.All<Movie>().SingleOrDefault();

            AllMoviesList.Value.Clear();

            movies.Results.ForEach((r)=>
            {
                AllMoviesList.Value.Add(r);
            });

            MainThread.BeginInvokeOnMainThread(()=>
            {
                ListVisible = true;
                IsRunning = false;
                IsEnabled = false;
            });
        }

        /// <summary>
        ///  Get movies from server and store them in cache.. with a TimeSpan limit.
        /// </summary>
        /// <returns>Bool if they are succesfully saved..</returns>
        public static async Task<bool> GetAndStoreMoviesAsync()
        {
            
            //Verify if internet connection is available
            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                var _conf = new MaterialSnackbarConfiguration()
                {
                    TintColor = Color.FromHex("#0066cc"),
                    BackgroundColor = Color.FromHex("#272B2E")
                };

                await MaterialDialog.Instance.SnackbarAsync("No internet Connection", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf);

                return false;
            }

            var token = new CancellationTokenSource();

            token.CancelAfter(4000);

            //var done = await ServiceLocator.Current.GetInstance<Lazy<ApiClient>>().Value.GetAndStoreMoviesAsync(false);
            var done = await Locator.Current.GetService<ApiClient>().GetAndStoreMoviesAsync(false);
            
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
                if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
                {
                    await MaterialDialog.Instance.SnackbarAsync("No internet Connection", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf);

                    return;
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ListVisible = false;
                    IsRunning = true;
                    IsEnabled = true;
                });

                var stored = await GetAndStoreMoviesAsync();

                if (!stored) 
                {
                    MainThread.BeginInvokeOnMainThread(async ()=>
                    {
                        //MsgVisible = true;
                        //MsgText = "Low storage left!";
                        await MaterialDialog.Instance.SnackbarAsync("Low storage.", "Dismiss", MaterialSnackbar.DurationIndefinite);
                        IsRunning = false;
                        IsEnabled = false;
                    });
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ListVisible = true;
                    IsEnabled = false;
                    IsRunning = false;
                });

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
            get => getStoreMoviesByGenresCommand ?? (getStoreMoviesByGenresCommand = new Command( async () =>
            {
                //Verify if internet connection is available
                if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
                {
                    await MaterialDialog.Instance.SnackbarAsync("No internet Connection", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf);

                    return;
                }

                FillMoviesByGenreList();
         
            }));
        }
        
        private Command getMoviesGenresCommand;
        public Command GetMoviesGenresCommand
        {
            get => getMoviesGenresCommand ?? (getMoviesGenresCommand = new Command(async () =>
            {
                await Task.Yield();
                //Verify if internet connection is available
                if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
                {
                    await MaterialDialog.Instance.SnackbarAsync("No internet Connection", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf);
                    return;
                }

                var done = await GetMoviesGenres();

                if (!done)
                {
                    MainThread.BeginInvokeOnMainThread(()=>
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
            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                await MaterialDialog.Instance.SnackbarAsync("No internet Connection", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf);
                return false;
            }
            
            return await Locator.Current.GetService<ApiClient>().GetAndStoreMovieGenresAsync();

        }

        private Command fillUpMoviesListAfterRefreshCommand;
        public Command FillUpMoviesListAfterRefreshCommand
        {
            get => fillUpMoviesListAfterRefreshCommand ?? (fillUpMoviesListAfterRefreshCommand = new Command( async () =>
            {

                //Verify if internet connection is available
                if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
                {
                    await MaterialDialog.Instance.SnackbarAsync("No internet Connection", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf);
                    return;
                }
                
                await FillMoviesList();

            }));
        }

        private Command fillUpMovies;
        public Command FillUpMovies 
        {
            get => fillUpMovies ?? (fillUpMovies = new Command(async () =>
            {
                await FillMoviesList();
            }));    
        }

        private Command noNetWorkHideTabs;
        public Command NoNetWorkHideTabs
        {
            get => noNetWorkHideTabs ?? (noNetWorkHideTabs = new Command(async () =>
            {
                //MessagingCenter.Send(this, "HIDE");
                await FillMoviesList();
                await MaterialDialog.Instance.SnackbarAsync("No internet Connection", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf);
             }));
        }

        public AllMoviesPageViewModel()
        {
            var realm = Realm.GetInstance();

            var movies = realm.All<Movie>().ToList();

            //Verify if internet connection is available
            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                return;
            }

            if (movies.Count < 1)
            {
                GetStoreMoviesCommand.Execute(null);
                GetMoviesGenresCommand.Execute(null);
            }
            else
            {
                FillUpMovies.Execute(null);
            }
            
        }
    }
}
