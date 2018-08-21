﻿using GBH_Movies_Test.Models;
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
using GBH_Movies_Test.Services;

namespace GBH_Movies_Test.ViewModels
{
    /// <summary>
    /// AllMoviesPage View Model
    /// </summary>
    public class AllMoviesPageViewModel : ViewModelBase
    {
        public ObservableCollection<Result> AllMoviesList { get; set; } = new ObservableCollection<Result>();

        private bool listVisible = false;
        public bool ListVisible
        {
            get => listVisible;
            set => SetProperty(ref listVisible, value);
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

        public void FillMoviesList()
        {
               
            var movies = Barrel.Current.Get<Movie>("Movies.Cached");

            foreach (var MovieResult in movies.Results)
            {
                var PosterPath = "https://image.tmdb.org/t/p/w370_and_h556_bestv2" + MovieResult.PosterPath;

                var Backdroppath = "https://image.tmdb.org/t/p/w1066_and_h600_bestv2" + MovieResult.BackdropPath;

                MovieResult.PosterPath = PosterPath;

                MovieResult.BackdropPath = Backdroppath;
                   
                AllMoviesList.Add(MovieResult);
            }

            ListVisible = true;

            ActivityIndicatorRunning = false;
        
        }
        /// <summary>
        ///  Get movies from server and store them in cache.. with a TimeSpan limit.
        /// </summary>
        /// <returns>Bool if they are succesfully saved..</returns>
        private static async Task<bool> GetAndStoreMoviesAsync() => await ServiceLocator.Current.GetInstance<ApiClient>().GetAndStoreMoviesAsync(true);

        private Command getStoreMoviesCommand;
        public Command GetStoreMoviesCommand
        {
            get => getStoreMoviesCommand ?? (getStoreMoviesCommand = new Command( async () =>
            {
                var stored = await GetAndStoreMoviesAsync();

                MoviesStored = stored;

                if (MoviesStored)
                {
                    FillMoviesList();
                }

            }));
        }

        private Command fillUpMoviesListAfterRefreshCommand;
        public Command FillUpMoviesListAfterRefreshCommand
        {
            get => fillUpMoviesListAfterRefreshCommand ?? (fillUpMoviesListAfterRefreshCommand = new Command(async () =>
            {

                FillMoviesList();

            }));
        }

        public AllMoviesPageViewModel()
        {
            //If the barrel cache doesn't exits or its expired.. Get the movies again and store them..
            if (!Barrel.Current.Exists("Movies.Cached") || Barrel.Current.IsExpired("Movies.Cached"))
            {
                GetStoreMoviesCommand.Execute(null);
            }
            else
            {
                ListVisible = false;

                ActivityIndicatorRunning = true;

                FillMoviesList();
            }
        }
    }
}
