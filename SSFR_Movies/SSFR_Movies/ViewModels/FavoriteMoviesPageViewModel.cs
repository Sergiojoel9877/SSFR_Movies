using System.Collections.ObjectModel;
using System.Linq;
using Splat;
using SSFR_Movies.Helpers;
using SSFR_Movies.Models;
using SSFR_Movies.Services.Abstract;
using Xamarin.Forms;

namespace SSFR_Movies.ViewModels
{
    /// <summary>
    /// FavoriteMoviesPage View Model
    /// </summary>
    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    public class FavoriteMoviesPageViewModel : ViewModelBase
    {

        IMovieService MovieService { get; set; }
         
        public ObservableCollection<Result> FavMoviesList { get; set; } = new();

        string pinSource = "Unpin.png";
        public string PinSource
        {
            get => pinSource;
            set => SetProperty(ref pinSource, value);
        }

        bool favImageIsVisible = true;
        public bool FavImageIsVisible
        {
            get => favImageIsVisible;
            set => SetProperty(ref favImageIsVisible, value);
        }

        bool messageIsVisible = true;
        public bool MessageIsVisible
        {
            get => messageIsVisible;
            set => SetProperty(ref messageIsVisible, value);
        }

        bool moviesListIsVisible = true;
        public bool MoviesListIsVisible
        {
            get => moviesListIsVisible;
            set => SetProperty(ref moviesListIsVisible, value);
        }

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

        public void FillMoviesList()
        {
            var realm = RealmDBSingleton.Current;

            var movies = realm.All<Result>().Where(x => x.FavoriteMovie == "Star.png").ToList();

            if (movies != null)
                foreach (var MovieResult in movies)
                    if (!FavMoviesList.Contains(MovieResult))
                        FavMoviesList.Add(MovieResult);
                
            FavImageIsVisible = FavMoviesList.Count() == 0;

            MessageIsVisible = FavImageIsVisible == true;

            MoviesListIsVisible = MessageIsVisible == false;
        }

        //public KeyValuePair<char, IEnumerable<Result>> FillMoviesList(IEnumerable<Result> results)
        //{
        //    var realm = RealmDBSingleton.Current;

        //    var movies = realm.All<Result>().Where(x => x.FavoriteMovie == "Star.png");

        //    if (movies.ToList().Count > 0)
        //    {
        //        return new KeyValuePair<char, IEnumerable<Result>>('r', movies); //Indica que la lista contiene elementos
        //    }
        //    return new KeyValuePair<char, IEnumerable<Result>>('v', movies); //Indica que la lista NO contiene elementos
        //}

        private Command getStoredMoviesCommand;
        public Command GetStoreMoviesCommand
        {
            get => getStoredMoviesCommand ??= new Command(() =>
            {
                FillMoviesList();
            });
        }

        public FavoriteMoviesPageViewModel()
        {
            if (FavMoviesList.Count == 0)
            {
                ListEmpty = true;
            }

            MovieService = Locator.Current.GetService<IMovieService>();

            MovieService.OnMovieAdded += MovieService_OnMovieAdded;

            MovieService.OnMovieRemoved += MovieService_OnMovieRemoved;

            GetStoreMoviesCommand.Execute(null);
        }

        public override void Dispose()
        {
            MovieService.OnMovieAdded -= MovieService_OnMovieAdded;
            MovieService.OnMovieAdded -= MovieService_OnMovieRemoved;
            MovieService = null;
            base.Dispose();
        }

        private void MovieService_OnMovieRemoved(object sender, MovieEventArgs e)
        {
            if (FavMoviesList.Contains(e.Result))
                FavMoviesList.Remove(e.Result);

            PinSource = "UnPin.png";

            FavImageIsVisible = FavMoviesList.Count() >= 0;

            MessageIsVisible = FavImageIsVisible == true;

            MoviesListIsVisible = MessageIsVisible == true;
        }

        private void MovieService_OnMovieAdded(object sender, MovieEventArgs e)
        {
            if(!FavMoviesList.Contains(e.Result))
                FavMoviesList.Add(e.Result);

            FavImageIsVisible = FavMoviesList.Count() == 0;

            MessageIsVisible = FavImageIsVisible == true;

            MoviesListIsVisible = MessageIsVisible == false;
        }
    }
}
