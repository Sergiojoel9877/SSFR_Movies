﻿using CommonServiceLocator;
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
using SSFR_Movies.Services;

namespace SSFR_Movies.ViewModels
{
    /// <summary>
    /// FavoriteMoviesPage View Model
    /// </summary>
  
    public class FavoriteMoviesPageViewModel : ViewModelBase
    {
       
        public ObservableCollection<Result> FavMoviesList { get; set; } = new ObservableCollection<Result>();

        private bool listVisible = false;
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

        public async Task<bool> FillMoviesList()
        {
           
            var movies = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().GetEntities();

            FavMoviesList.Clear();

            foreach (var MovieResult in movies)
            {
                if (FavMoviesList.Contains(MovieResult))
                {
                    return false;
                }

                FavMoviesList.Add(MovieResult);
            }

            ListVisible = true;

            ActivityIndicatorRunning = false;
     
            return true;
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
            if (FavMoviesList.Count == 0)
            {
                ListEmpty = true;
            }
           
            GetStoreMoviesCommand.Execute(null);
        }
    }
}