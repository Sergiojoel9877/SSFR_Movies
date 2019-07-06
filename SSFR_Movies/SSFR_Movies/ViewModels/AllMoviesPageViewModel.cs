using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Realms;
using Splat;
using SSFR_Movies.Models;
using SSFR_Movies.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
//using ReactiveUI.Legacy;
using XF.Material.Forms.UI.Dialogs;
using XF.Material.Forms.UI.Dialogs.Configurations;

namespace SSFR_Movies.ViewModels
{
    /// <summary>
    /// AllMoviesPage View Model
    /// </summary>
    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    public class AllMoviesPageViewModel : ViewModelBase
    {
        public Lazy<ObservableCollection<Result>> AllMoviesList { get; set; } = new Lazy<ObservableCollection<Result>>(() => new ObservableCollection<Result>());

        public Lazy<ObservableCollection<Genre>> GenreList { get; set; } = new Lazy<ObservableCollection<Genre>>(() => new ObservableCollection<Genre>());

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

        private bool isEnabled = false;
        public bool IsEnabled
        {
            get => isEnabled;
            set => SetProperty(ref isEnabled, value);
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

        async Task FillGenresList()
        {
            var realm = await Realm.GetInstanceAsync();

            var genreList = realm.All<Genres>().FirstOrDefault();

            genreList.GenresGenres.ForEach((g) =>
            {
                GenreList.Value.Add(g);
            });
        }

        public async Task<object> FillMoviesList()
        {
            var tcs = new TaskCompletionSource<object>();

            //Verify if internet connection is available
            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                await MaterialDialog.Instance.SnackbarAsync("No internet Connection", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf);
                tcs.SetResult(null);
                return tcs.Task;
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
                tcs.SetResult(null);
                return tcs.Task;
            }

            movies.Results.ForEach((r) =>
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
            tcs.SetResult(null);
            return tcs.Task;
        }

        public void FillMoviesByGenreList()
        {

            //Verify if internet connection is available
            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await MaterialDialog.Instance.SnackbarAsync("No internet Connection", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf);
                });

                return;
            }
            var realm = Realm.GetInstance();

            var movies = realm.All<Movie>().SingleOrDefault();

            AllMoviesList.Value.Clear();

            movies.Results.ForEach((r) =>
            {
                AllMoviesList.Value.Add(r);
            });

            Device.BeginInvokeOnMainThread(() =>
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

        private AsyncCommand getStoreMoviesCommand;
        public AsyncCommand GetStoreMoviesCommand
        {
            get => getStoreMoviesCommand ?? (getStoreMoviesCommand = new AsyncCommand(async () =>
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
                   IsRunning = true;
                   IsEnabled = true;
               });

               var stored = await GetAndStoreMoviesAsync();

               if (!stored)
               {
                   Device.BeginInvokeOnMainThread(async () =>
                   {
                        //MsgVisible = true;
                        //MsgText = "Low storage left!";
                        await MaterialDialog.Instance.SnackbarAsync("Low storage.", "Dismiss", MaterialSnackbar.DurationIndefinite);
                       IsRunning = false;
                       IsEnabled = false;
                   });
               }

               Device.BeginInvokeOnMainThread(() =>
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

        private AsyncCommand getStoreMoviesByGenresCommand;
        public AsyncCommand GetStoreMoviesByGenresCommand
        {
            get => getStoreMoviesByGenresCommand ?? (getStoreMoviesByGenresCommand = new AsyncCommand(async () =>
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

        private AsyncCommand getMoviesGenresCommand;
        public AsyncCommand GetMoviesGenresCommand
        {
            get => getMoviesGenresCommand ?? (getMoviesGenresCommand = new AsyncCommand(async () =>
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
                    Device.BeginInvokeOnMainThread(() =>
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

        private AsyncCommand fillUpMoviesListAfterRefreshCommand;
        public AsyncCommand FillUpMoviesListAfterRefreshCommand
        {
            get => fillUpMoviesListAfterRefreshCommand ?? (fillUpMoviesListAfterRefreshCommand = new AsyncCommand(async () =>
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

        private AsyncCommand fillUpMovies;
        public AsyncCommand FillUpMovies
        {
            get => fillUpMovies ?? (fillUpMovies = new AsyncCommand(async () =>
            {
                await FillMoviesList();
                await FillGenresList();
            }));
        }

        private AsyncCommand noNetWorkHideTabs;
        public AsyncCommand NoNetWorkHideTabs
        {
            get => noNetWorkHideTabs ?? (noNetWorkHideTabs = new AsyncCommand(async () =>
            {
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
                GetStoreMoviesCommand.ExecuteAsync().SafeFireAndForget();
                GetMoviesGenresCommand.ExecuteAsync().SafeFireAndForget();
            }
            else
            {
                FillUpMovies.ExecuteAsync().SafeFireAndForget();
            }
        }
    }
}
