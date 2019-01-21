using CommonServiceLocator;
using Plugin.Connectivity;
using SSFR_Movies.Data;
using SSFR_Movies.Helpers;
using SSFR_Movies.Models;
using SSFR_Movies.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.Views
{
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MovieDetailsPage : ContentPage
    {
        public MovieDetailsPage(Result movie)
        {
            InitializeComponent();
            
            var tap = new TapGestureRecognizer();

            tap.Tapped += TitleTapped;

            PosterPathImage.GestureRecognizers.Add(tap);

            IsPresentInFavList(movie);

            AddToFav.Source = "StarEmpty.png";

            BindingContext = movie;

            Scroll.TranslationX = -500;

            QuitFromFavLayout.Clicked += QuitFromFavorites;
            
            AddToFavLayout.Clicked += Tap_Tapped;

            SetImagesContent();

            if (movie.Title.Length >= 25)
            {
                MovieTitle.SetAnimation();
            }
        }

        public void SetImagesContent()
        {
            Device.BeginInvokeOnMainThread(async ()=>
            {
                var item = BindingContext as Result;

                Uri bimg, pimg;

                Uri.TryCreate("https://image.tmdb.org/t/p/w1066_and_h600_bestv2" + item.BackdropPath, UriKind.Absolute, out bimg);

                Uri.TryCreate("https://image.tmdb.org/t/p/w370_and_h556_bestv2" + item.PosterPath, UriKind.Absolute, out pimg);

                Task<ImageSource> bimg_result = Task<ImageSource>.Factory.StartNew(() => ImageSource.FromUri(bimg));

                Task<ImageSource> pimg_result = Task<ImageSource>.Factory.StartNew(() => ImageSource.FromUri(pimg));

                PosterPathImage.Source = await pimg_result.ConfigureAwait(false);

                BackDropImage.Source = await bimg_result.ConfigureAwait(false);

            });
        }

        private async void IsPresentInFavList(Result m)
        {

            var movieExists = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().EntityExits(m.Id);

            if (movieExists)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    AddToFav.Source = "Star.png";

                    AddToFavLayout.IsVisible = false;

                    QuitFromFavLayout.IsVisible = true;
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    AddToFavLayout.IsVisible = true;

                    QuitFromFavLayout.IsVisible = false;
                });
            }
        }

        private async void Tap_Tapped(object sender, EventArgs e)
        {
            await Task.Yield();

            await AddToFav.ScaleTo(1.50, 500, Easing.SpringOut);

            await AddToFav.ScaleTo(1, 500, Easing.SpringIn);

            await AddToFavList();
        }

        private async Task AddToFavList()
        {
            await Task.Yield();

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
                        AddToFav.Source = "Star.png";
                    }
                    else
                    {
                        var addMovie = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().AddEntity(movie);

                        if (addMovie)
                        {
                            
                            await SpeakNow("Added Successfully"); //NOT COMPATIBLE WITH ANDROID 9.0 AT THE MOMENT.

                            await DisplayAlert("Added Successfully", "The movie " + movie.Title + " was added to your favorite list!", "ok");

                            MessagingCenter.Send(this, "Refresh", true);

                            Device.BeginInvokeOnMainThread(()=>
                            {
                                AddToFav.Source = "Star.png";

                                AddToFavLayout.IsVisible = false;

                                QuitFromFavLayout.IsVisible = true;
                            });

                        }
                    } 
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error: " + e.InnerException);
                }
            }
            else
            {
                Settings.UpdateList = false;
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

                    var deleteMovie = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().DeleteEntity(movie);

                    if (deleteMovie)
                    {
                        await DisplayAlert("Deleted Successfully", "The movie " + movie.Title + " was deleted from your favorite list!", "ok");

                        AddToFav.Source = "StarEmpty.png";

                        AddToFavLayout.IsVisible = true;

                        QuitFromFavLayout.IsVisible = false;

                        MessagingCenter.Send(this, "Refresh", true);

                        //Settings.UpdateList = true;
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
                Settings.UpdateList = false;
            }
        }
        
        protected async override void OnAppearing()
        {

            base.OnAppearing();
            
            var item = BindingContext as Result;

            IsPresentInFavList(item);

            var t3 = ScrollTrailer.ScrollToAsync(-200, 0, true);

            var t4 = Scroll.TranslateTo(0, 0, 1500, Easing.SpringOut);
            
            await Task.WhenAll(t3, t4);

            if (!CrossConnectivity.Current.IsConnected)
            {
                DependencyService.Get<IToast>().LongAlert("No internet conection, try later..");
                return;
            }

            var movie = (Result)BindingContext;

            var video = await ServiceLocator.Current.GetInstance<Lazy<ApiClient>>().Value.GetMovieVideosAsync((int)movie.Id);

            if (video.Results.Count() == 0)
            {
                ScrollTrailer.IsVisible = false;
            }
            else
            {
                ScrollTrailer.IsVisible = true;
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

        private async void PlayTrailer_Tapped(object sender, EventArgs e)
        {
            await Task.Yield();

            var movie = (Result)BindingContext;

            var video = await ServiceLocator.Current.GetInstance<Lazy<ApiClient>>().Value.GetMovieVideosAsync((int)movie.Id);

            Device.OpenUri(new Uri("vnd.youtube://watch/" + video.Results.Where(v => v.Type == "Trailer").FirstOrDefault().Key));
        }

        private void StreamMovie_Tapped(object sender, EventArgs e)
        {

            var item = BindingContext as Result;

            //streamWV.IsVisible = true;

            //streamWV.Source = ServiceLocator.Current
            //    .GetInstance<Lazy<ApiClient>>().Value
            //    .PlayMovieByNameAndYear(item.Title.Replace(" ", "+").Replace(":", String.Empty),
            //    item.ReleaseDate.Substring(0, 4));

            Device.OpenUri(new Uri(ServiceLocator.Current.GetInstance<Lazy<ApiClient>>().Value
                                                                                        .PlayMovieByNameAndYear(item.Title.Replace(" ", "+").Replace(":", String.Empty),
                                                                                        item.ReleaseDate.Substring(0, 4))));
            streamWV.GoBack();
        }

        private async void TitleTapped(object sender, EventArgs e)
        {
           await PosterPathImage.ScaleTo(1.3, 500, Easing.Linear);

           await PosterPathImage.ScaleTo(1, 500, Easing.Linear);
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            if (streamWV.CanGoBack)
            {
                streamWV.GoBack();
            }
        }

        private void FWButton_Clicked(object sender, EventArgs e)
        {
            if (streamWV.CanGoForward)
            {
                streamWV.GoForward();
            }
        }
    }
}