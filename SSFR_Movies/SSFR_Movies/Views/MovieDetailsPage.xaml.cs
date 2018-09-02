using CommonServiceLocator;
using Plugin.Connectivity;
using SSFR_Movies.Data;
using SSFR_Movies.Models;
using SSFR_Movies.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MovieDetailsPage : ContentPage
    {
        TapGestureRecognizer tap = null;
        TapGestureRecognizer quitTap = null;

        public MovieDetailsPage(Result movie)
        {
            InitializeComponent();

            IsPresentInFavList(movie);

            BindingContext = movie;

            tap = new TapGestureRecognizer();

            tap.Tapped += Tap_Tapped;

            quitTap = new TapGestureRecognizer();

            quitTap.Tapped += QuitFromFavorites;

            QuitFromFavLayout.GestureRecognizers.Add(quitTap);

            AddToFavLayout.GestureRecognizers.Add(tap);

        }

        private async void IsPresentInFavList(Result m)
        {

            var movieExists = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().EntityExits(m.Id);

            if (movieExists)
            {
                AddToFavLayout.IsVisible = false;

                QuitFromFavLayout.IsVisible = true;

                Device.BeginInvokeOnMainThread(() =>
                {
                    AddToFav.Source = "Star.png";
                });
            }
            else
            {
                AddToFavLayout.IsVisible = true;

                QuitFromFavLayout.IsVisible = false;
            }
        }

        private async void Tap_Tapped(object sender, EventArgs e)
        {
            await Task.Yield();

            var t = AddToFav.ScaleTo(1.50, 250, Easing.SpringOut);

            var t2 = AddToFav.ScaleTo(1, 500, Easing.SpringIn);

            var t3 = AddToFavList();

            await Task.WhenAll(t, t2, t3);
        }

        private async Task AddToFavList()
        {

            var movie = (Result)BindingContext;

            //Verify if internet connection is available
            if (!CrossConnectivity.Current.IsConnected)
            {
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");
                    return false;
                });
                return;
            }

            if (await DisplayAlert("Suggestion", "Would you like to add this movie to your favorites list?", "Yes", "No"))
            {
                try
                {
                    var movieExists = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().EntityExits(movie.Id);

                    if (movieExists)
                    {
                        await DisplayAlert("Oh no!", "It looks like " + movie.Title + " already exits in your favorite list!", "ok");

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            AddToFav.Source = "Star.png";
                        });

                    }

                    var addMovie = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().AddEntity(movie);

                    if (addMovie)
                    {
                        await DisplayAlert("Added Successfully", "The movie " + movie.Title + " was added to your favorite list!", "ok");

                        await SpeakNow("Added Successfully");

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            AddToFav.Source = "Star.png";
                        });

                        AddToFavLayout.IsVisible = false;

                        QuitFromFavLayout.IsVisible = true;
                    }
                }
                catch (Exception)
                {

                }
            }
            else
            {

            }
        }

        private async void QuitFromFavorites(object sender, EventArgs e)
        {
            var movie = (Result)BindingContext;

            //Verify if internet connection is available
            if (!CrossConnectivity.Current.IsConnected)
            {
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");
                    return false;
                });
                return;
            }

            if (await DisplayAlert("Suggestion", "Would you like to delete this movie from your favorites list?", "Yes", "No"))
            {
                try
                {

                    //var deleteMovie = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().DeleteEntity(movie);

                    var deleteMovie = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().DeleteEntity(movie);

                    if (deleteMovie)
                    {
                        await DisplayAlert("Deleted Successfully", "The movie " + movie.Title + " was deleted from your favorite list!", "ok");
                        
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            AddToFav.Source = "StarEmpty.png";

                            AddToFavLayout.IsVisible = true;

                            QuitFromFavLayout.IsVisible = false;
                        });
                    }
                }
                catch (Exception)
                {
                    Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                    {
                        DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection or maybe that movie doesn't exists!");

                        return false;
                    });
                }
            }
            else
            {

            }
        }
        
        protected async override void OnAppearing()
        {

            base.OnAppearing();

            var item = BindingContext as Result;

            IsPresentInFavList(item);

            var t3 = ScrollTrailer.ScrollToAsync(-200, 0, true);
            
            await Task.WhenAll(t3);

            var movie = (Result)BindingContext;

            var video = await ServiceLocator.Current.GetInstance<ApiClient>().GetMovieVideosAsync((int)movie.Id);

            if (video.Results.Count() == 0)
            {
                ScrollTrailer.IsVisible = false;
            }
            else
            {
                ScrollTrailer.IsVisible = true;
            }

        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
            GC.Collect(0, GCCollectionMode.Optimized, false);
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

        private async void PlayTrailer_Tapped(object sender, EventArgs e)
        {
            await Task.Yield();

            var movie = (Result)BindingContext;

            var video = await ServiceLocator.Current.GetInstance<ApiClient>().GetMovieVideosAsync((int)movie.Id);

            Device.OpenUri(new Uri("vnd.youtube://watch/" + video.Results.Where(v => v.Type == "Trailer").FirstOrDefault().Key));
        }

        private async void TitleTapped(object sender, EventArgs e)
        {
            var tilte = ((Label)sender).Text;

            if (tilte.Length > 25)
            {
                var t = PosterPath.FadeTo(0, 500, Easing.Linear);

                var t3 = PosterPath.FadeTo(1, 1000, Easing.Linear);

                await Task.WhenAll(t, t3);
            }
        }
    }
}