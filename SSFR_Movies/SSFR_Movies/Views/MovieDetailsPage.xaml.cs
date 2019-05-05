using Realms;
using Splat;
//using SSFR_Movies.Data;
using SSFR_Movies.Helpers;
using SSFR_Movies.Models;
using SSFR_Movies.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.Views
{
    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MovieDetailsPage : ContentPage
    {
        //string RegexOpenLoad = "https?:\\/\\/(www\\.)?(openload|oload)\\.[^\\/,^\\.]{2,}\\/(embed|f)\\/.+";

        public MovieDetailsPage(Result movie)
        {
            InitializeComponent();

            vScroll.TranslationX = -500;

            Result resultSingleton = ResultSingleton.Instance();

            BindingContext = resultSingleton;

            //MessagingCenter.Subscribe<AllMoviesPage, Result>(this, "Result", (p, e)=>
            //{
            //    BindingContext = p;
            //});

            MessagingCenter.Send(this, "ClearSelection");

            SetPosterImgGestureRecognizer();

            SetAddSourceToFavoritesListImage("StarEmpty.png");

            SetEventHandlers();

            SetAnimationToMovieTitleIfTitleIsGreaterThan25Chars(movie);

            LowerChildInAbsoluteLayout();


        }

        private void SetAnimationToMovieTitleIfTitleIsGreaterThan25Chars(Result movie)
        {
            if (movie.Title.Length >= 25)
            {
                MovieTitle.SetAnimation();
            }
        }

        private void LowerChildInAbsoluteLayout()
        {
            absoluteLayout.LowerChild(theFrame);
        }

        private void SetEventHandlers()
        {
            QuitFromFavLayout.Clicked += QuitFromFavorites;

            AddToFavLayout.Clicked += Tap_Tapped;
        }

        private void SetAddSourceToFavoritesListImage(string v)
        {
            AddToFav.Source = "StarEmpty.png";
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

            vScroll.TranslationX = 500;

            Result resultSingleton = ResultSingleton.Instance();
      
            BindingContext = resultSingleton;

            SetAnimationToMovieTitleIfTitleIsGreaterThan25Chars(resultSingleton);

            var item = BindingContext as Result;

            IsPresentInFavList(item);

            Task.Run(async ()=>
            {
                await ScrollTrailer.ScrollToAsync(-200, 0, true);
            });

            MessagingCenter.Send(this, "ClearSelection");

            SetPosterImgGestureRecognizer();

            SetAddSourceToFavoritesListImage("StarEmpty.png");

            SetEventHandlers();

            LowerChildInAbsoluteLayout();
        }

        //string Check(string regexpttrn, string uri)
        //{
        //    var regex = new Regex(@regexpttrn, RegexOptions.Compiled);
        //    Match match = regex.Match(uri);
        //    return match.Success ? match.Value : "";
        //}

        private async void IsPresentInFavList(Result m)
        {

            var realm = await Realm.GetInstanceAsync();

            var movieExists = realm.Find<Result>(m.Id);

            if (movieExists != null && movieExists.FavoriteMovie == "Star.png")
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    AddToFav.Source = "Star.png";

                    AddToFavLayout.IsVisible = false;

                    QuitFromFavLayout.IsVisible = true;
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
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

            await AddToFavList();

            await AddToFav.ScaleTo(1, 500, Easing.SpringIn);
        }

        private async Task AddToFavList()
        {
            await Task.Yield();

            var movie = (Result)BindingContext;

            var realm = await Realm.GetInstanceAsync();

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

            if (await DisplayAlert("Suggestion", "Would you like to add this movie to your favorites list?", "Yes", "no"))
            {
                try
                {
                    var movieExists = realm.Find<Result>(movie.Id);

                    if (movieExists != null && movieExists.FavoriteMovie == "Star.png")
                    {
                        await DisplayAlert("Oh no!", "It looks like " + movie.Title + " already exits in your favorite list!", "ok");
                        AddToFav.Source = "Star.png";
                    }
                    else
                    {

                        realm.Write(() =>
                        {
                            movie.FavoriteMovie = "Star.png";

                            realm.Add(movie, true);
                        });

                        await SpeakNow("Added Successfully");

                        await DisplayAlert("Added Successfully", "The movie " + movie.Title + " was added to your favorite list!", "ok");

                        MessagingCenter.Send(this, "Refresh", true);

                        MessagingCenter.Send(this, "ClearSelection");

                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            AddToFav.Source = "Star.png";

                            AddToFavLayout.IsVisible = false;

                            QuitFromFavLayout.IsVisible = true;
                        });
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error: " + e.InnerException);
                }
            }
        }

        private async void QuitFromFavorites(object sender, EventArgs e)
        {
            var movie = (Result)BindingContext;

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

            if (await DisplayAlert("Suggestion", "Would you like to delete this movie from your favorites list?", "Yes", "No"))
            {
                try
                {
                    var realm = await Realm.GetInstanceAsync();

                    realm.Write(() =>
                    {
                        movie.FavoriteMovie = "StarEmpty.png";

                        realm.Add(movie, true);
                    });

                    await DisplayAlert("Deleted Successfully", "The movie " + movie.Title + " was deleted from your favorite list!", "ok");

                    AddToFav.Source = "StarEmpty.png";

                    AddToFavLayout.IsVisible = true;

                    QuitFromFavLayout.IsVisible = false;

                    MessagingCenter.Send(this, "Refresh", true);
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
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            MainThread.BeginInvokeOnMainThread(async ()=>
            {
                await vScroll.TranslateTo(0, 0, 1000, Easing.Linear);
            });
    
            MessagingCenter.Send(this, "ClearSelection");

            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                DependencyService.Get<IToast>().LongAlert("No internet conection, try later..");
                return;
            }

            var movie = (Result)BindingContext;

            var video = await Locator.Current.GetService<ApiClient>().GetMovieVideosAsync(movie.Id);

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

            var video = await Locator.Current.GetService<ApiClient>().GetMovieVideosAsync(movie.Id);

            if (video.Results.Count() > 0)
            {
                Device.OpenUri(new Uri("vnd.youtube://watch/" + video.Results.Where(v => v.Type == "Trailer").FirstOrDefault().Key));
            }
            else
            {
                DependencyService.Get<IToast>().LongAlert("No trailers for this movie/serie found");
            }
        }

        private async void StreamMovie_Tapped(object sender, EventArgs e)
        {

            var item = BindingContext as Result;

            streamWV.IsVisible = true;

            await vScroll.ScrollToAsync(0, 500, true);

            var URI = Locator
                        .Current
                            .GetService<ApiClient>()
                                .PlayMovieByNameAndYear(item.Title.Replace(" ", "+").Replace(":", String.Empty),
                                    item.ReleaseDate.Substring(0, 4));

            streamWV.Source = URI;

            streamWV.Navigated += StreamWV_Navigated;

            streamWVswap.Navigated += StreamWVswap_Navigated;
        }

        private void StreamWVswap_Navigated(object sender, WebNavigatedEventArgs e)
        {
            try
            {
                var nav = (WebView)sender;

                if (!e.Url.StartsWith("https://openloed.co"))
                {
                    nav.GoBack();
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        private void StreamWV_Navigated(object sender, WebNavigatedEventArgs e)
        {
            try
            {
                var nav = (WebView)sender;

                if (!e.Url.StartsWith("https://videospider.in"))
                {
                    if (e.Url.StartsWith("https://openloed.co"))
                    {
                        streamWV.IsVisible = false;
                        streamWVswap.IsVisible = true;

                        //string encoded = "LyoKICAgICAgICB4R2V0dGVyCiAgICAgICAgICBCeQogICAgS2h1biBIdGV0eiBOYWluZyBbZmIu\n" +
                        //        "Y29tL0tIdGV0ek5haW5nXQpSZXBvID0+IGh0dHBzOi8vZ2l0aHViLmNvbS9LaHVuSHRldHpOYWlu\n" +
                        //        "Zy94R2V0dGVyCgoqLwoKdmFyIG9wZW5sb2FkID0gL2h0dHBzPzpcL1wvKHd3d1wuKT8ob3Blbmxv\n" +
                        //        "YWR8b2xvYWQpXC5bXlwvLF5cLl17Mix9XC8oZW1iZWR8ZilcLy4rL2ksCiAgICBzdHJlYW0gPSAv\n" +
                        //        "aHR0cHM/OlwvXC8od3d3XC4pPyhzdHJlYW1hbmdvfGZydWl0c3RyZWFtc3xzdHJlYW1jaGVycnl8\n" +
                        //        "ZnJ1aXRhZGJsb2NrfGZydWl0aG9zdHMpXC5bXlwvLF5cLl17Mix9XC8oZnxlbWJlZClcLy4rL2ks\n" +
                        //        "CiAgICBtZWdhdXAgPSAvaHR0cHM/OlwvXC8od3d3XC4pPyhtZWdhdXApXC5bXlwvLF5cLl17Mix9\n" +
                        //        "XC8uKy9pLAogICAgbXA0dXBsb2FkID0gL2h0dHBzPzpcL1wvKHd3d1wuKT9tcDR1cGxvYWRcLlte\n" +
                        //        "XC8sXlwuXXsyLH1cL2VtYmVkXC0uKy9pLAogICAgc2VuZHZpZCA9IC9odHRwcz86XC9cLyh3d3dc\n" +
                        //        "Lik/KHNlbmR2aWQpXC5bXlwvLF5cLl17Mix9XC8uKy9pLAogICAgdmlkY2xvdWQgPSAvaHR0cHM/\n" +
                        //        "OlwvXC8od3d3XC4pPyh2aWRjbG91ZHx2Y3N0cmVhbXxsb2FkdmlkKVwuW15cLyxeXC5dezIsfVwv\n" +
                        //        "ZW1iZWRcLyhbYS16QS1aMC05XSopL2ksCiAgICByYXBpZHZpZGVvID0gL2h0dHBzPzpcL1wvKHd3\n" +
                        //        "d1wuKT9yYXBpZHZpZGVvXC5bXlwvLF5cLl17Mix9XC8oXD92PVteJlw/XSp8ZVwvLit8dlwvLisp\n" +
                        //        "L2k7CgppZiAob3BlbmxvYWQudGVzdCh3aW5kb3cubG9jYXRpb24uaHJlZikpIHsKICAgIHhHZXR0\n" +
                        //        "ZXIuZnVjayhkb2N1bWVudC5sb2NhdGlvbi5wcm90b2NvbCArICcvLycgKyBkb2N1bWVudC5sb2Nh\n" +
                        //        "dGlvbi5ob3N0ICsgJy9zdHJlYW0vJyArIGRvY3VtZW50LmdldEVsZW1lbnRCeUlkKCJEdHNCbGtW\n" +
                        //        "RlF4IikudGV4dENvbnRlbnQgKyAnP21pbWU9dHJ1ZScpOwp9IGVsc2UgaWYgKHN0cmVhbS50ZXN0\n" +
                        //        "KHdpbmRvdy5sb2NhdGlvbi5ocmVmKSkgewogICAgeEdldHRlci5mdWNrKHdpbmRvdy5sb2NhdGlv\n" +
                        //        "bi5wcm90b2NvbCArIHNyY2VzWzBdWyJzcmMiXSk7Cn0gZWxzZSBpZiAobWVnYXVwLnRlc3Qod2lu\n" +
                        //        "ZG93LmxvY2F0aW9uLmhyZWYpKSB7CiAgICBzZWNvbmRzID0gMDsKICAgIGRpc3BsYXkoKTsKICAg\n" +
                        //        "IHdpbmRvdy5sb2NhdGlvbi5yZXBsYWNlKGRvY3VtZW50LmdldEVsZW1lbnRzQnlDbGFzc05hbWUo\n" +
                        //        "ImJ0biBidG4tZGVmYXVsdCIpLml0ZW0oMCkuaHJlZik7Cn0gZWxzZSBpZiAobXA0dXBsb2FkLnRl\n" +
                        //        "c3Qod2luZG93LmxvY2F0aW9uLmhyZWYpKSB7CiAgICB4R2V0dGVyLmZ1Y2soZG9jdW1lbnQuZ2V0\n" +
                        //        "RWxlbWVudHNCeUNsYXNzTmFtZSgnanctdmlkZW8ganctcmVzZXQnKS5pdGVtKDApLnNyYyk7Cn0g\n" +
                        //        "ZWxzZSBpZiAocmFwaWR2aWRlby50ZXN0KHdpbmRvdy5sb2NhdGlvbi5ocmVmKSkgewogICAgeEdl\n" +
                        //        "dHRlci5mdWNrKGRvY3VtZW50LmdldEVsZW1lbnRzQnlUYWdOYW1lKCdzb3VyY2UnKS5pdGVtKDAp\n" +
                        //        "LnNyYyk7Cn0gZWxzZSBpZiAoc2VuZHZpZC50ZXN0KHdpbmRvdy5sb2NhdGlvbi5ocmVmKSkgewog\n" +
                        //        "ICAgeEdldHRlci5mdWNrKGRvY3VtZW50LmdldEVsZW1lbnRzQnlUYWdOYW1lKCdzb3VyY2UnKS5p\n" +
                        //        "dGVtKDApLnNyYyk7Cn0gZWxzZSBpZiAodmlkY2xvdWQudGVzdCh3aW5kb3cubG9jYXRpb24uaHJl\n" +
                        //        "ZikpIHsKICAgICQuYWpheCh7CiAgICAgICAgdXJsOiAnL2Rvd25sb2FkJywKICAgICAgICBtZXRo\n" +
                        //        "b2Q6ICdQT1NUJywKICAgICAgICBkYXRhOiB7CiAgICAgICAgICAgIGZpbGVfaWQ6IGZpbGVJRAog\n" +
                        //        "ICAgICAgIH0sCiAgICAgICAgZGF0YVR5cGU6ICdqc29uJywKICAgICAgICBzdWNjZXNzOiBmdW5j\n" +
                        //        "dGlvbihyZXMpIHsKICAgICAgICAgICAgJCgnLnF1YWxpdHktbWVudScpLmh0bWwocmVzLmh0bWwp\n" +
                        //        "OwogICAgICAgICAgICB2YXIgZGF0YSA9IHJlcy5odG1sOwogICAgICAgICAgICB2YXIgcmVnZXgg\n" +
                        //        "PSAvaHJlZj0iKC4qPykiLzsKICAgICAgICAgICAgdmFyIG07CiAgICAgICAgICAgIGlmICgobSA9\n" +
                        //        "IHJlZ2V4LmV4ZWMoZGF0YSkpICE9PSBudWxsKSB7CiAgICAgICAgICAgICAgICB4R2V0dGVyLmZ1\n" +
                        //        "Y2sobVsxXSk7CiAgICAgICAgICAgIH0KICAgICAgICB9CiAgICB9KTsKfSBlbHNlIGlmKHdpbmRv\n" +
                        //        "dy5sb2NhdGlvbi5ob3N0ID09ICdkcml2ZS5nb29nbGUuY29tJyl7Cglkb2N1bWVudC5nZXRFbGVt\n" +
                        //        "ZW50QnlJZCgndWMtZG93bmxvYWQtbGluaycpLmNsaWNrKCk7Cn0KCi8qClN1cHBvcnRlZCBTaXRl\n" +
                        //        "cwo9PiBPcGVubG9hZCAoQWxsIGRvbWFpbnMpCj0+IEZydWl0U3RyZWFtcyAoU3RyZWFtY2hlcnJ5\n" +
                        //        "LFN0cmVhbWFuZ28gYW5kIGV0Yy4uKQo9PiBNcDRVcGxvYWQKPT4gUmFwaWRWaWRlbwo9PiBTZW5k\n" +
                        //        "VmlkCj0+IE1lZ2FVcAo9PiBWaWRDbG91ZCAoQWxsIGRvbWFpbnMpCiov";

                        //streamWV.Eval("(function(){" +
                        //        "var parent = document.getElementByTagName('head').item(0);" +
                        //        "var script = document.createElement('script');" +
                        //        "script.type ='text/jasvascript';" +
                        //        //Decode the BASE64 using he browser
                        //        "script.innerHTML = window.atob('" + encoded + "');" +
                        //        "parent.appendChild(script)" +
                        //        "})()");
                        streamWVswap.Source = new Uri(e.Url);
                        //new HtmlWebViewSource()
                        //{
                        //    Html = $@"<html><body>
                        //            <frame src='{e.Url}'>
                        //            </body></html>"
                        //};
                        //Task.Run(async ()=>
                        //{
                        //    await Browser.OpenAsync(new Uri(e.Url));
                        //});
                        //var stream = Check(RegexOpenLoad, e.Url);
                        Device.OpenUri(new Uri(e.Url));
                    }
                }
            }
            catch (Exception er)
            {
                Debug.WriteLine(er.Message);
            }
        }

        private async void TitleTapped(object sender, EventArgs e)
        {
            await PosterPathImage.ScaleTo(1.3, 500, Easing.Linear);

            await PosterPathImage.ScaleTo(1, 500, Easing.Linear);
        }
    }
}