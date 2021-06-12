using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using FFImageLoading;
using Realms;
using Splat;
using SSFR_Movies.Helpers;
using SSFR_Movies.Models;
using SSFR_Movies.Services;
using SSFR_Movies.Services.Abstract;
using StructLinq;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
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
        public ObservableRangeCollection<Result> AllMoviesList { get; set; } = new();

        public Lazy<ObservableRangeCollection<Genre>> GenreList { get; set; } = new();

        IMovieService MovieService { get; set; } = Locator.Current.GetService<IMovieService>();

        private Result result = new();
        public Result Result
        {
            get => result;
            set => SetProperty(ref result, value);
        }

        readonly Lazy<MaterialSnackbarConfiguration> _conf = new(() => new MaterialSnackbarConfiguration()
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

        void FillGenresList()
        {
            var genreList = RealmDB.All<Genres>().FirstOrDefault();

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
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await MaterialDialog.Instance.SnackbarAsync("No internet Connection", "Dismiss", MaterialSnackbar.DurationIndefinite, _conf.Value);
                });

                tcs.SetResult(null);
                return tcs.Task;
            }
            
            var movies = RealmDB.All<Movie>().SingleOrDefault();
            
            if (movies == null)
            {
                tcs.SetResult(null);
                return tcs.Task;
            }

            AllMoviesList.AddRange(movies.Results);

            tcs.SetResult(null);
            return tcs.Task;
        }

        public async Task<object> FillMoviesByGenreList()
        {
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

            var movies = RealmDB.All<Movie>().SingleOrDefault();

            AllMoviesList.Clear();

            AllMoviesList.AddRange(movies.Results);

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

            return await Locator.Current.GetService<ApiClient>().GetAndStoreMoviesAsync(false);
        }

        private Lazy<AsyncCommand> remainingItemsThresholdReachedCommand;
        public Lazy<AsyncCommand> RemainingItemsThresholdReachedCommand
        {
            get => remainingItemsThresholdReachedCommand ??= new Lazy<AsyncCommand>(() => new AsyncCommand(async () => await ProcessRemainingItemsThresholdReachedCommand()));
        }

        async Task ProcessRemainingItemsThresholdReachedCommand()
        {
            var Semaphore = new SemaphoreSlim(1, 1);

            await Semaphore.WaitAsync();

            ImageService.Instance.SetPauseWork(true);

            try
            {
                Settings.NextPage++;

                var MoviesDownloaded = await Locator.Current.GetService<ApiClient>().GetAndStoreMoviesAsync(false, page: Settings.NextPage);

                if (MoviesDownloaded)
                {
                    await FillUpMoviesListAfterRefreshCommand.Value.ExecuteAsync();
                    ImageService.Instance.SetPauseWork(false);
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
            get => getStoreMoviesCommand ??= new Lazy<AsyncCommand>(() => new AsyncCommand(async () => await ProcessStoreMovies()));
        }

        private AsyncCommand<Result> addToFavListCommand;
        public AsyncCommand<Result> AddToFavListCommand
        {
            get => addToFavListCommand ??= new AsyncCommand<Result>(async (r) =>
            {
                Result = r;
                await MovieService.AddMovieToFavoritesList(Result);
            });
        }

        private AsyncCommand<Result> navToDetailsPage;
        public AsyncCommand<Result> NavToDetailsPage
        {
            get => navToDetailsPage ??= new AsyncCommand<Result>(async (r) =>
            {
                Result = r;
                ResultSingleton.SetInstance(Result);
                await Shell.Current.GoToAsync("/MovieDetails", true);
            });
        }

        async Task ProcessStoreMovies()
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                ListVisible = false;
                IsRunning = true;
                IsEnabled = true;
            });

            var stored = await Locator.Current.GetService<ApiClient>().GetAndStoreMoviesAsync(false);

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
            get => getStoreMoviesByGenresCommand ??= new Lazy<AsyncCommand>(() => new AsyncCommand(async () => await FillMoviesByGenreList()));
        }

        private Lazy<AsyncCommand> getMoviesGenresCommand;
        public Lazy<AsyncCommand> GetMoviesGenresCommand
        {
            get => getMoviesGenresCommand ??= new Lazy<AsyncCommand>(() => new AsyncCommand(async ()=> await ProcessGenres()));
        }

        async Task ProcessGenres()
        {
            var done = await Locator.Current.GetService<ApiClient>().GetAndStoreMovieGenresAsync();

            if (!done)
            {
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    MsgVisible = true;
                    MsgText = "No storage space left!";
                });
            }
        }

        private Lazy<AsyncCommand> fillUpMoviesListAfterRefreshCommand;
        public Lazy<AsyncCommand> FillUpMoviesListAfterRefreshCommand
        {
            get => fillUpMoviesListAfterRefreshCommand ??= new Lazy<AsyncCommand>(() => new AsyncCommand(async () => await FillMoviesList()));
        }

        private Lazy<AsyncCommand> fillUpMovies;
        public Lazy<AsyncCommand> FillUpMovies
        {
            get => fillUpMovies ??= new Lazy<AsyncCommand>(() => new AsyncCommand(async () =>
            {
                await FillMoviesList();
                FillGenresList();
            }));
        }

        private Lazy<AsyncCommand> noNetWorkHideTabs;
        public Lazy<AsyncCommand> NoNetWorkHideTabs
        {
            get => noNetWorkHideTabs ??= new Lazy<AsyncCommand>(() => new AsyncCommand(async () => await FillMoviesList()));
        }

        public Realm RealmDB { get; private set; } = Realm.GetInstance();

        public AllMoviesPageViewModel()
        {
            try
            {
                var movies = RealmDB.All<Movie>().Count();

                //Verify if internet connection is available
                if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
                {
                    return;
                }

                if (movies < 1)
                {
                    GetStoreMoviesCommand.Value.ExecuteAsync().SafeFireAndForget(true);
                    GetMoviesGenresCommand.Value.ExecuteAsync().SafeFireAndForget(true);
                }
                else
                {
                    FillUpMovies.Value.ExecuteAsync().SafeFireAndForget(true);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
        }
    }
}
