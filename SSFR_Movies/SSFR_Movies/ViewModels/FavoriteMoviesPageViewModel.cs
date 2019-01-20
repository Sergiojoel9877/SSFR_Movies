using CommonServiceLocator;
using SSFR_Movies.Data;
using SSFR_Movies.Models;
using MonkeyCache.FileStore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using SSFR_Movies.Helpers;
using SSFR_Movies.Services;

namespace SSFR_Movies.ViewModels
{
    /// <summary>
    /// FavoriteMoviesPage View Model
    /// </summary>
    [Preserve(AllMembers = true)]
    public class FavoriteMoviesPageViewModel : ViewModelBase
    {
       
        public Lazy<ObservableCollection<Result>> FavMoviesList { get; set; } = new Lazy<ObservableCollection<Result>>(()=> new ObservableCollection<Result>());

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

            var movies = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().GetEntities().ConfigureAwait(false);

            FavMoviesList.Value.Clear();

            foreach (var MovieResult in movies)
            {
                if (!FavMoviesList.Value.Contains(MovieResult))
                {
                    FavMoviesList.Value.Add(MovieResult);
                }
            }

            if (FavMoviesList.Value.Count == 0)
            {
                return 'v'; //Indica que la lista esta vacia
            }

            return 'r'; //Indica que la lista contiene elementos
            
        }

        private Command getStoredMoviesCommand;
        public Command GetStoreMoviesCommand
        {
            get => getStoredMoviesCommand ?? (getStoredMoviesCommand = new Command(async () =>
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
           
            GetStoreMoviesCommand.Execute(null);
        }
    }
}
