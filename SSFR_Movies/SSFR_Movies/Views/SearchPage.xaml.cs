﻿using Splat;
using SSFR_Movies.Helpers;
using SSFR_Movies.Models;
using SSFR_Movies.Services;
using SSFR_Movies.ViewModels;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.Views
{
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        AllMoviesPageViewModel vm;

        public SearchPage()
        {
            InitializeComponent();

            vm = Locator.Current.GetService<AllMoviesPageViewModel>();

            activityIndicator.IsVisible = false;

            BindingContext = vm;
            
            Shell.SetNavBarIsVisible(this, false);

            searchBar.Focus();

            SuscribeToMessages();

            MoviesList.SelectionChangedCommand = new Command(MovieSelected);
            
        }

        private void SuscribeToMessages()
        {
       
            //MessagingCenter.Subscribe<CustomViewCell>(this, "_PushAsync", (s) =>
            //{
            //    MovieSelected();
            //});

            MessagingCenter.Subscribe<MovieDetailsPage>(this, "ClearSelection", (e) =>
            {
                 MoviesList.SelectedItem = null;
            });
        }

        private void MovieSelected()
        {
            if (MoviesList.SelectedItem != null)
            {
                var movie = MoviesList.SelectedItem as Result;

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PushAsync(new MovieDetailsPage(movie));
                });
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            SuscribeToMessages();
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

            MainThread.BeginInvokeOnMainThread(async () =>
            {

                try
                {
                    if (key != "")
                    {

                        var movie_results = await Locator.CurrentMutable.GetService<ApiClient>().SearchMovieByName(key);
                        
                        if (movie_results.Results.Count != 0)
                        {

                            vm.AllMoviesList.Value.Clear();

                            foreach (var MovieResult in movie_results.Results)
                            {
                                vm.AllMoviesList.Value.Add(MovieResult);
                            }

                            BindingContext = vm;

                            MoviesList.IsVisible = true;

                            MoviesList.ItemsSource = vm.AllMoviesList.Value;

                            await MoviesList.TranslateTo(0, 0, 500, Easing.SpringIn);
                            
                            activityIndicator.IsVisible = false;

                            activityIndicator.IsRunning = false;

                            await SpeakNow("Search completed");

                        }
                        else
                        {
                           
                            MoviesList.ItemsSource = null;

                            activityIndicator.IsVisible = false;

                            activityIndicator.IsRunning = false;

                            MoviesList.IsVisible = true;

                            DependencyService.Get<IToast>().LongAlert("It seems like that movie doesn't exists, check your spelling!");

                            await SpeakNow("It seems like that movie doesn't exists, check your spelling!");

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

        private async void ItemSelected(object sender, ItemTappedEventArgs e)
        {
            try
            {

                if (e.Item == null)
                {
                    return;
                }

                var movie = (Result)e.Item;

                ((ListView)sender).SelectedItem = null;
                
                await Navigation.PushAsync(new MovieDetailsPage(movie), true);

            }
            catch (Exception e4)
            {
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("An error has ocurred!");
                    Debug.WriteLine("Error: " + e4.InnerException);
                    Vibration.Vibrate();

                    return false;
                });
            }
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
                SearchBoxVisibility = SearchBoxVisiblity.Collapsable;
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