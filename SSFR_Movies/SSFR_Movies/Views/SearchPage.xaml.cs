﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Splat;
using SSFR_Movies.Helpers;
using SSFR_Movies.Models;
using SSFR_Movies.Services;
using SSFR_Movies.Services.Abstract;
using SSFR_Movies.ViewModels;
using SSFR_Movies.Views.DataTemplateSelectors;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.Views
{
    [Preserve(AllMembers = true)]
    public partial class SearchPage : ContentPage
    {
        readonly AllMoviesPageViewModel vm;

        public SearchPage()
        {
            InitializeComponent();

            vm = Locator.Current.GetService<AllMoviesPageViewModel>();

            BindingContext = vm;

            activityIndicator.IsVisible = false;
            
            Shell.SetNavBarIsVisible(this, false);

            searchBar.Focus();

            SetListItemTemplate();

            SetListOrientationLayout();
        }

        private void SetListItemTemplate()
        {
            MoviesList.ItemTemplate = new SelectedMovieTemplateSelector();
            MoviesList.Bind(CollectionView.ItemsSourceProperty, nameof(AllMoviesPageViewModel.AllMoviesList));
        }

        private void SetListOrientationLayout()
        {
            MoviesList.ItemsLayout = new GridItemsLayout(2, ItemsLayoutOrientation.Vertical);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            await Task.Yield();

            activityIndicator.IsVisible = true;

            activityIndicator.IsRunning = true;

            MoviesList.IsVisible = false;

            var key = ((SearchBar)sender).Text;

            if (key == "")
            {
                DependencyService.Get<IToast>().LongAlert("The name can't be empty");

                await SpeakNow("The name can't be empty");

                return;
            }

            //Verify if internet connection is available
            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");
                    return false;
                });
                return;
            }

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    if (key != "")
                    {

                        var movie_results = await Locator.Current.GetService<ApiClient>().SearchMovieByName(key);

                        if (movie_results.Results.Count != 0)
                        {

                            vm.AllMoviesList.Clear();

                            foreach (var MovieResult in movie_results.Results)
                            {
                                if (MovieResult.BackdropPath != null)
                                {
                                    vm.AllMoviesList.Add(MovieResult);
                                }
                            }

                            BindingContext = vm;

                            MoviesList.IsVisible = true;

                            MoviesList.ItemsSource = vm.AllMoviesList;

                            await MoviesList.TranslateTo(0, 0, 500, Easing.SpringIn);

                            activityIndicator.IsVisible = false;

                            activityIndicator.IsRunning = false;

                            //await SpeakNow("Search completed");

                        }
                        else
                        {

                            MoviesList.ItemsSource = null;

                            activityIndicator.IsVisible = false;

                            activityIndicator.IsRunning = false;

                            MoviesList.IsVisible = true;

                            DependencyService.Get<IToast>().LongAlert("It seems like that movie doesn't exists, check your spelling!");

                            //await SpeakNow("It seems like that movie doesn't exists, check your spelling!");

                            Vibration.Vibrate();

                        }
                    }
                }
                catch (Exception e3)
                {
                    Debug.WriteLine("Error: " + e3.InnerException);
                }
            });
        }

        public async Task SpeakNow(string msg)
        {
            var settings = new SpeechOptions()
            {
                Volume = 1f,
                Pitch = 1.0f
            };

            await TextToSpeech.SpeakAsync(msg, settings);
        }

        private async void Back_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync(true);
        }

        public class MovieSearchHandler : SearchHandler
        {
            public MovieSearchHandler()
            {
                SearchBoxVisibility = SearchBoxVisibility.Collapsible;
                IsSearchEnabled = true;
                Placeholder = "Search";
            }

            protected override void OnQueryConfirmed()
            {
                base.OnQueryConfirmed();
            }

            protected override void OnQueryChanged(string oldValue, string newValue)
            {
                // Do nothing, we will wait for confirmation

            }
        }
    }
}