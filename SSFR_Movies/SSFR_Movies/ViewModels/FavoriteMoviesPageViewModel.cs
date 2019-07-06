//using CommonServiceLocator;
//using SSFR_Movies.Data;
using AsyncAwaitBestPractices.MVVM;
using Realms;
using SSFR_Movies.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSFR_Movies.ViewModels
{
    /// <summary>
    /// FavoriteMoviesPage View Model
    /// </summary>
    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    public class FavoriteMoviesPageViewModel : ViewModelBase
    {

        public Lazy<ObservableCollection<Result>> FavMoviesList { get; set; } = new Lazy<ObservableCollection<Result>>(() => new ObservableCollection<Result>(), isThreadSafe: true);

        private bool listVisible = true;
        public bool ListVisible
        {
            get => listVisible;
            set => SetProperty(ref listVisible, value);
        }

        private bool activityIndicatorRunning = true;
        public bool ActivityIndicatorRunning
        {
            get => activityIndicatorRunning;
            set => SetProperty(ref activityIndicatorRunning, value);
        }

        private bool listEmpty = true;
        public bool ListEmpty
        {
            get => listEmpty;
            set => SetProperty(ref listEmpty, value);
        }

        public async Task<char> FillMoviesList()
        {
            await Task.Yield();

            var realm = await Realm.GetInstanceAsync();

            var movies = realm.All<Result>().Where(x => x.FavoriteMovie == "Star.png");

            if (movies != null)
            {
                foreach (var MovieResult in movies)
                {
                    if (!FavMoviesList.Value.Contains(MovieResult))
                    {
                        FavMoviesList.Value.Add(MovieResult);
                    }
                }
            }

            if (FavMoviesList.Value.Count == 0)
            {
                return 'v'; //Indica que la lista esta vacia
            }

            return 'r'; //Indica que la lista contiene elementos

        }
        public async Task<KeyValuePair<char, IEnumerable<Result>>> FillMoviesList(IEnumerable<Result> results)
        {
            await Task.Yield();

            var realm = await Realm.GetInstanceAsync();

            var movies = realm.All<Result>().Where(x => x.FavoriteMovie == "Star.png");

            if (movies.ToList().Count > 0)
            {
                return new KeyValuePair<char, IEnumerable<Result>>('r', movies); //Indica que la lista contiene elementos
            }
            else
            {
                return new KeyValuePair<char, IEnumerable<Result>>('v', movies); //Indica que la lista NO contiene elementos
            }
        }

        private AsyncCommand getStoredMoviesCommand;
        public AsyncCommand GetStoreMoviesCommand
        {
            get => getStoredMoviesCommand ?? (getStoredMoviesCommand = new AsyncCommand(async () =>
            {
                await FillMoviesList();
            }));
        }

        public FavoriteMoviesPageViewModel()
        {
            if (FavMoviesList.Value.Count == 0)
            {
                ListEmpty = true;
            }

            GetStoreMoviesCommand.ExecuteAsync();
        }
    }
}
