using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Splat;
//using SSFR_Movies.Data;
using SSFR_Movies.Helpers;
using SSFR_Movies.Models;
using SSFR_Movies.Services;
using SSFR_Movies.Services.Abstract;
using SSFR_Movies.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.Views
{
    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MovieDetailsPage : ContentPage
    {
        int Count { get; set; } = 1;
        Result Result { get; set; }
        private void SetAnimationToMovieTitleIfTitleIsGreaterThan25Chars(Result movie)
        {
            if (movie?.Title?.Length >= 25)
            {
                Device.BeginInvokeOnMainThread(()=>
                {
                    var Go = new Animation(d => MovieTitle.TranslationX = d, 350, -1000);

                    Go.Commit(MovieTitle, "Animation", 250, 9000, Easing.Linear, (d, b) =>
                    {
                        MovieTitle.TranslationX = 100;
                    }, () => true);
                });
            }
        }

        private void LowerChildInAbsoluteLayout()
        {
            absoluteLayout2.LowerChild(theFrame2);
            absoluteLayout.RaiseChild(AddToFavLayout);
        }

        private void SetPosterImgGestureRecognizer()
        {
            var tap = new TapGestureRecognizer();

            tap.Tapped += TitleTapped;

            PosterPathImage.GestureRecognizers.Add(tap);
        }

        public MovieDetailsPage()
        {
            InitializeComponent();

            Initialize();
        }

        void Initialize()
        {
            BindingContext = Locator.Current.GetService<MovieDetailsPageViewModel>();
            (BindingContext as MovieDetailsPageViewModel).OnMovieAddedRemovedUpdateUIElements += MovieDetailsPage_OnMovieAddedRemovedUpdateUIElements;
        }

        private async void MovieDetailsPage_OnMovieAddedRemovedUpdateUIElements(object sender, EventArgs e)
        {
            await Task.WhenAll(
                AddToFav.ScaleTo(1.50, 500, Easing.SpringOut),
                AddToFav.ScaleTo(1, 500, Easing.SpringIn));
        }

        protected override void OnAppearing()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                DependencyService.Get<IToast>().LongAlert("No internet conection, try later..");
                return;
            }

            Result = ResultSingleton.GetInstance();

            SetAnimationToMovieTitleIfTitleIsGreaterThan25Chars(Result);

            (BindingContext as MovieDetailsPageViewModel).IsPresentInFavList().ContinueWith(w => { });

            SetPosterImgGestureRecognizer();

            LowerChildInAbsoluteLayout();

            if (Result.Id != 0)
            {
                var video = new MovieVideo();

                Locator.Current.GetService<ApiClient>().GetMovieVideosAsync(Result.Id).ContinueWith(async r =>
                {
                    video = await r;
                });

                if (video.Results.Count() == 0)
                {
                    PlayTrailer.ScaleTo(0, 250, Easing.Linear);
                }
                else
                {
                    PlayTrailer.ScaleTo(1, 250, Easing.Linear);
                }
            }
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            (BindingContext as MovieDetailsPageViewModel).OnMovieAddedRemovedUpdateUIElements -= MovieDetailsPage_OnMovieAddedRemovedUpdateUIElements;
            base.OnDisappearing();
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

        private async void StreamMovie_Tapped(object sender, EventArgs e)
        {
            var item = BindingContext as Result;

            var URI = Locator
                        .Current
                            .GetService<ApiClient>()
                                .PlayMovieByNameAndYear(item.Title.Replace(" ", "+").Replace(":", String.Empty),
                                    item.ReleaseDate.Substring(0, 4));

            await Xamarin.Essentials.Launcher.TryOpenAsync(new Uri(URI));
        }

        private async void TitleTapped(object sender, EventArgs e)
        {
            await PosterPathImage.ScaleTo(1.3, 500, Easing.Linear);

            await PosterPathImage.ScaleTo(1, 500, Easing.Linear);
        }

        private void HScroll_Scrolled(object sender, ScrolledEventArgs e)
        {
            var scroll = (ScrollView)sender;
            double scrollingSpace = scroll.ContentSize.Height - scroll.Height;

            var title = new Label
            {
                Text = Result.Title,
                Scale = 0,
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                FontSize = 20,
                VerticalTextAlignment = TextAlignment.Center
            };

            MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await title.ScaleTo(1, 1500, Easing.Linear);
            }).SafeFireAndForget();

            if (scrollingSpace <= e.ScrollY)
            {
                if (Count == 1)
                {
                    MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await AddToFavLayout.ScaleTo(1, 250, Easing.Linear);
                    }).SafeFireAndForget();

                    Shell.SetTitleView(this, null);
                    Shell.SetTitleView(this, title);
                }
                Count += 1;
            }
        }
    }
}