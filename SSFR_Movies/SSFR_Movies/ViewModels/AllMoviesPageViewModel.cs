using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using MvvmHelpers;
using Realms;
using Sharpnado.Tasks;
using Splat;
using SSFR_Movies.Helpers;
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
        public Lazy<ObservableRangeCollection<Result>> AllMoviesList { get; set; } = new Lazy<ObservableRangeCollection<Result>>(() => new ObservableRangeCollection<Result>());

        public Lazy<ObservableRangeCollection<Genre>> GenreList { get; set; } = new Lazy<ObservableRangeCollection<Genre>>(() => new ObservableRangeCollection<Genre>());

        readonly Lazy<MaterialSnackbarConfiguration> _conf = new Lazy<MaterialSnackbarConfiguration>(() => new MaterialSnackbarConfiguration()
        {
            TintColor = Color.FromHex("#0066cc"),
            BackgroundColor = Color.FromHex("#272B2E")
        });

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
            await Task.Yield();

            var tcs = new TaskCompletionSource<object>();

            //Verify if internet connection is available
            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await MaterialDialog.Instance.SnackbarAsync("No internet Connection", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf.Value);
                });

                tcs.SetResult(null);
                return tcs.Task;
            }

            await Device.InvokeOnMainThreadAsync(() =>
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

            ////await Device.InvokeOnMainThreadAsync(()=>
            ////{
            //    movies.Results.ForEach((r) =>
            //    {
            //        AllMoviesList.Value.Add(r);
            //    });
            ////});

            AllMoviesList.Value.AddRange(movies.Results);
           
            await Device.InvokeOnMainThreadAsync(() =>
            {
                ListVisible = true;
                MsgVisible = false;
                IsEnabled = false;
                IsRunning = false;
            });

            tcs.SetResult(null);
            return tcs.Task;
        }

        public async Task<object> FillMoviesByGenreList()
        {
            await Task.Yield();

            var tcs = new TaskCompletionSource<object>();
            //Verify if internet connection is available
            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await MaterialDialog.Instance.SnackbarAsync("No internet Connection", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf.Value);
                });

                tcs.SetResult(null);
                return await tcs.Task;
            }

            var realm = Realm.GetInstance();
            var movies = realm.All<Movie>().SingleOrDefault();

            AllMoviesList.Value.Clear();

            //movies.Results.ForEach((r) =>
            //{
            //    AllMoviesList.Value.Add(r);
            //});

            AllMoviesList.Value.AddRange(movies.Results);

            await Device.InvokeOnMainThreadAsync(() =>
            {
                ListVisible = true;
                IsRunning = false;
                IsEnabled = false;
            });

            tcs.SetResult(null);
            return tcs.Task;
        }

        /// <summary>
        ///  Get movies from server and store them in cache.. with a TimeSpan limit.
        /// </summary>
        /// <returns>Bool if they are succesfully saved..</returns>
        [Obsolete("This wrapper is obsolete")]
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

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await MaterialDialog.Instance.SnackbarAsync("No internet Connection", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf);
                });

                return false;
            }

            //var done = await Locator.Current.GetService<ApiClient>().GetAndStoreMoviesAsync(false);

            //if (done)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            return await Locator.Current.GetService<ApiClient>().GetAndStoreMoviesAsync(false);
        }

        private Lazy<AsyncCommand> remainingItemsThresholdReachedCommand;
        public Lazy<AsyncCommand> RemainingItemsThresholdReachedCommand
        {
            get => remainingItemsThresholdReachedCommand ?? (remainingItemsThresholdReachedCommand = new Lazy<AsyncCommand>(() => new AsyncCommand(()=> ProcessRemainingItemsThresholdReachedCommand())));
        }

        async Task ProcessRemainingItemsThresholdReachedCommand()
        {
            var Semaphore = new SemaphoreSlim(1, 1);

            await Semaphore.WaitAsync();

            try
            {
                Settings.NextPage++;

                var MoviesDownloaded = await Locator.Current.GetService<ApiClient>().GetAndStoreMoviesAsync(false, page: Settings.NextPage);

                if (MoviesDownloaded)
                {
                    await FillUpMoviesListAfterRefreshCommand.Value.ExecuteAsync();
                }
            }
            finally
            {
                Semaphore.Release();
            }
        }

        private Lazy<AsyncCommand> getStoreMoviesCommand;
        public Lazy<AsyncCommand> GetStoreMoviesCommand
        {
            get => getStoreMoviesCommand ?? (getStoreMoviesCommand = new Lazy<AsyncCommand>(() => new AsyncCommand(()=> ProcessStoreMovies())));
        }
        
        async Task ProcessStoreMovies()
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                ListVisible = false;
                IsRunning = true;
                IsEnabled = true;
            });

            var stored = /*await GetAndStoreMoviesAsync();*/ await Locator.Current.GetService<ApiClient>().GetAndStoreMoviesAsync(false);

            if (!stored)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await MaterialDialog.Instance.SnackbarAsync("Low storage.", "Dismiss", MaterialSnackbar.DurationIndefinite);
                    IsRunning = false;
                    IsEnabled = false;
                });
            }

            await Device.InvokeOnMainThreadAsync(() =>
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
        }

        private Lazy<AsyncCommand> getStoreMoviesByGenresCommand;
        public Lazy<AsyncCommand> GetStoreMoviesByGenresCommand
        {
            get => getStoreMoviesByGenresCommand ?? (getStoreMoviesByGenresCommand = new Lazy<AsyncCommand>(() => new AsyncCommand(()=> FillMoviesByGenreList())));
        }

        private Lazy<AsyncCommand> getMoviesGenresCommand;
        public Lazy<AsyncCommand> GetMoviesGenresCommand
        {
            get => getMoviesGenresCommand ?? (getMoviesGenresCommand = new Lazy<AsyncCommand>(() => new AsyncCommand(()=> ProcessGenres())));
        }

        async Task ProcessGenres()
        {
            var done = /*await GetMoviesGenres();*/ await Locator.Current.GetService<ApiClient>().GetAndStoreMovieGenresAsync();

            if (!done)
            {
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    MsgVisible = true;
                    MsgText = "No storage space left!";
                });
            }
        }

        [Obsolete("This wrapper is currently obsolete")]
        private async Task<bool> GetMoviesGenres()
        {
            await Task.Yield();

            //Verify if internet connection is available
            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await MaterialDialog.Instance.SnackbarAsync("No internet Connection", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf.Value);
                });
                return false;
            }

            return await Locator.Current.GetService<ApiClient>().GetAndStoreMovieGenresAsync();
        }

        private Lazy<AsyncCommand> fillUpMoviesListAfterRefreshCommand;
        public Lazy<AsyncCommand> FillUpMoviesListAfterRefreshCommand
        {
            get => fillUpMoviesListAfterRefreshCommand ?? (fillUpMoviesListAfterRefreshCommand = new Lazy<AsyncCommand>(() => new AsyncCommand(()=> FillMoviesList())));
        }

        private Lazy<AsyncCommand> fillUpMovies;
        public Lazy<AsyncCommand> FillUpMovies
        {
            get => fillUpMovies ?? (fillUpMovies = new Lazy<AsyncCommand>(() => new AsyncCommand(()=> Task.WhenAll(FillMoviesList(), FillGenresList()))));
        }

        private Lazy<AsyncCommand> noNetWorkHideTabs;
        public Lazy<AsyncCommand> NoNetWorkHideTabs
        {
            get => noNetWorkHideTabs ?? (noNetWorkHideTabs = new Lazy<AsyncCommand>(() => new AsyncCommand(()=> FillMoviesList())));
        }

        public AllMoviesPageViewModel()
        {
            var realm = Realm.GetInstance();

            var movies = realm.All<Movie>().Count();

            //Verify if internet connection is available
            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                return;
            }

            if (movies < 1)
            {
                TaskMonitor.Create(Task.WhenAll(GetStoreMoviesCommand.Value.ExecuteAsync(), GetMoviesGenresCommand.Value.ExecuteAsync()));
            }
            else
            {
                TaskMonitor.Create(FillUpMovies.Value.ExecuteAsync());
            }
        }
    }
}
