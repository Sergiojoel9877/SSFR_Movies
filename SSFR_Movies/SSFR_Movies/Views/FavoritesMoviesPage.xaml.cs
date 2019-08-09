﻿//using CommonServiceLocator;
using AsyncAwaitBestPractices.MVVM;
using Realms;
using Splat;
using SSFR_Movies.Helpers;
//using SSFR_Movies.Data;
using SSFR_Movies.Models;
using SSFR_Movies.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.Views
{
    /// <summary>
    /// FavoriteMoviesPage Code Behind
    /// </summary>
    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
   
    public partial class FavoritesMoviesPage : ContentPage
    {
        readonly FavoriteMoviesPageViewModel vm;
        readonly ToolbarItem searchToolbarItem = null;

        public FavoritesMoviesPage()
        {
            InitializeComponent();

            vm = Locator.Current.GetService<FavoriteMoviesPageViewModel>();

            BindingContext = vm;

            SetVisibility();

            UnPin.Source = "Unpin.png";

            searchToolbarItem = new ToolbarItem()
            {
                Text = "Search",
                IconImageSource = "Search.png",
                Priority = 0,

                Command = new AsyncCommand(async () =>
                {
                    await Shell.Current.GoToAsync("/Search", false);
                })
            };

            ToolbarItems.Add(searchToolbarItem);

            //MoviesList.SelectionChangedCommand = new Command(MovieSelected);

            SubscribeToMessage();
        }

        private void SubscribeToMessage()
        {
            MessagingCenter.Subscribe<MovieDetailsPage, bool>(this, "Refresh", (s, e) =>
            {
                if (e)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        var estado = await vm.FillMoviesList(null);

                        if (estado.Key == 'r')
                        {
                            MoviesList.IsVisible = true;
                            UnPin.IsVisible = false;
                            Message.IsVisible = false;
                            MoviesList.ItemsSource = estado.Value;
                            MoviesList.SelectedItem = null;
                        }
                        else if (estado.Key == 'v')
                        {
                            MoviesList.IsVisible = false;
                            UnPin.IsVisible = true;
                            Message.IsVisible = true;
                            MoviesList.SelectedItem = null;
                        }
                    });
                }
            });

            MessagingCenter.Subscribe<CustomViewCellFavPage, bool>(this, "Refresh", (s, e) =>
            {
                if (e)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        var estado = await vm.FillMoviesList(null);

                        if (estado.Key == 'r')
                        {
                            MoviesList.IsVisible = true;
                            UnPin.IsVisible = false;
                            Message.IsVisible = false;
                            MoviesList.ItemsSource = estado.Value;
                            MoviesList.SelectedItem = null;
                        }
                        else if (estado.Key == 'v')
                        {
                            MoviesList.IsVisible = false;
                            UnPin.IsVisible = true;
                            Message.IsVisible = true;
                            MoviesList.SelectedItem = null;
                        }
                    });
                }
            });

            MessagingCenter.Subscribe<MovieDetailsPage>(this, "ClearSelection", (e) =>
            {
                MoviesList.SelectedItem = null;
            });

            MessagingCenter.Subscribe<CustomViewCell, bool>(this, "Refresh", (s, e) =>
            {
                if (e)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        var estado = await vm.FillMoviesList(null);

                        if (estado.Key == 'r')
                        {
                            MoviesList.IsVisible = true;
                            UnPin.IsVisible = false;
                            Message.IsVisible = false;
                            MoviesList.ItemsSource = estado.Value;
                            MoviesList.SelectedItem = null;
                        }
                        else if (estado.Key == 'v')
                        {
                            MoviesList.IsVisible = false;
                            UnPin.IsVisible = true;
                            Message.IsVisible = true;
                            MoviesList.SelectedItem = null;
                        }
                    });
                }
            });
        }

        private async void MovieSelected(object sender, SelectionChangedEventArgs e)
        {
            await ResultSingleton.SetInstanceAsync(MoviesList.SelectedItem as Result);

            await Shell.Current.GoToAsync("/MovieDetails", false);
            MoviesList.SelectedItem = null;
        }

        private async void SetVisibility()
        {
            var realm = await Realm.GetInstanceAsync();

            var movies_db = realm.All<Result>().Where(x => x.FavoriteMovie == "Star.png").ToList();

            UnPin.IsVisible = movies_db.Count() == 0 ? true : false;

            Message.IsVisible = UnPin.IsVisible == true ? true : false;

            MoviesList.IsVisible = Message.IsVisible == true ? false : true;
        }

        private async void QuitVisibility()
        {
            var realm = await Realm.GetInstanceAsync();

            var movies_db = realm.All<Result>().ToList();

            UnPin.IsVisible = movies_db.Count() != 0 ? false : true;

            Message.IsVisible = UnPin.IsVisible == true ? true : false;

            MoviesList.IsVisible = Message.IsVisible == true ? false : true;

        }

        /// <summary>
        /// To animate the Quit from Favorite list icon..
        /// </summary>
        /// <param name="sender">the incoming object </param>
        /// <param name="e">the event arguments in that object</param>
        private async void UnPin_Tapped(object sender, EventArgs e)
        {
            var img = sender as Image;

            await img.ScaleTo(2, 500, Easing.BounceOut);

            await img.ScaleTo(1, 250, Easing.BounceIn);
        }
    }
}
