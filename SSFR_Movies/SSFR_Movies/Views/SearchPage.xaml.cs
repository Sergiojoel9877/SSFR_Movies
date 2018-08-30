﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonServiceLocator;
using Plugin.Connectivity;
using SSFR_Movies.Models;
using SSFR_Movies.Services;
using SSFR_Movies.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        AllMoviesPageViewModel vm;

        public SearchPage()
        {
            InitializeComponent();

            vm = new AllMoviesPageViewModel();

            BindingContext = vm;

            searchBar.Focus();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            BindingContext = vm;

            searchBar.Focus();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            BindingContext = null;

        }
        
        private async void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {

            await Task.Yield();

            activityIndicator.IsVisible = true;

            activityIndicator.IsRunning = true;

            var key = ((SearchBar)sender).Text;

            if (key == "")
            {
                DependencyService.Get<IToast>().LongAlert("The name can't be empty");

                await SpeakNow("The name can't be empty");

                return;
            }

            MoviesList.BeginRefresh();

            //Verify if internet connection is available
            if (!CrossConnectivity.Current.IsConnected)
            {
                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");
                    return false;
                });
                return;
            }

            Device.BeginInvokeOnMainThread(async () =>
            {

                try
                {

                    if (key != "")
                    {

                        var movie_results = await App.ApiClient.SearchMovieByName(key);

                        if (movie_results.Results.Capacity != 0)
                        {

                            vm.AllMoviesList.Clear();

                            foreach (var MovieResult in movie_results.Results)
                            {
                                var PosterPath = "https://image.tmdb.org/t/p/w370_and_h556_bestv2" + MovieResult.PosterPath;

                                var Backdroppath = "https://image.tmdb.org/t/p/w1066_and_h600_bestv2" + MovieResult.BackdropPath;

                                MovieResult.PosterPath = PosterPath;

                                MovieResult.BackdropPath = Backdroppath;

                                vm.AllMoviesList.Add(MovieResult);

                            }

                            BindingContext = vm;

                            MoviesList.ItemsSource = vm.AllMoviesList;

                            await MoviesList.TranslateTo(0, 0, 500, Easing.SpringIn);

                            MoviesList.EndRefresh();
                            
                            activityIndicator.IsVisible = false;

                            activityIndicator.IsRunning = false;
                            
                            await SpeakNow("Search completed");

                        }
                        else
                        {
                            
                            MoviesList.EndRefresh();

                            MoviesList.ItemsSource = null;

                            activityIndicator.IsVisible = false;

                            activityIndicator.IsRunning = false;

                            DependencyService.Get<IToast>().LongAlert("It seems like that movie doesn't exists, check your spelling!");

                            await SpeakNow("It seems like that movie doesn't exists, check your spelling!");

                            Vibration.Vibrate();

                        }
                    }
                }
                catch (Exception e3)
                {

                    MoviesList.EndRefresh();

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
                
                await Navigation.PushAsync(new MovieDetailsPage(movie));

            }
            catch (Exception e4)
            {
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("An error has ocurred!");

                    Vibration.Vibrate();

                    return false;
                });
            }
        }

        public async Task SpeakNow(string msg)
        {
            var settings = new SpeakSettings()
            {
                Volume = 1f,
                Pitch = 1.0f
            };

            await TextToSpeech.SpeakAsync(msg, settings);
        }

        private async void Back_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync(false);
        }
    }
}