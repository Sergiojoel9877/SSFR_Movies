using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Realms;
using Splat;
using SSFR_Movies.Helpers;
using SSFR_Movies.Models;
using SSFR_Movies.Services;
using SSFR_Movies.Services.Abstract;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSFR_Movies.ViewModels
{
    public class MovieDetailsPageViewModel : ViewModelBase
    {

        public event EventHandler OnMovieAddedRemovedUpdateUIElements;

        string addToFavSource = "StarEmpty.png";//"Star.png";
        public string AddToFavSource
        {
            get => addToFavSource;
            set => SetProperty(ref addToFavSource, value);
        }

        bool addToFavLayoutIsVisible = true;
        public bool AddToFavLayoutIsVisible
        {
            get => addToFavLayoutIsVisible;
            set => SetProperty(ref addToFavLayoutIsVisible, value);
        }

        bool quitFromFavLayoutIsVisible = false;
        public bool QuitFromFavLayoutIsVisible
        {
            get => quitFromFavLayoutIsVisible;
            set => SetProperty(ref quitFromFavLayoutIsVisible, value);
        }

        Result result = new();
        public Result Result
        {
            get => result;
            set => SetProperty(ref result, value);
        }

        Realm RealmDB { get; set; }

        IMovieService MovieService { get; set; }

        AsyncCommand addToFavListCommand;
        public AsyncCommand AddToFavListCommand
        {
            get => addToFavListCommand ??= new(async ()=>
            {
                if (await Shell.Current.DisplayAlert("Suggestion", "Would you like to add this movie to your favorites list?", "Yes", "no"))
                {
                    var movieResult = RealmDB.Find<Result>(Result.Id);

                    if (movieResult != null && movieResult.FavoriteMovie == "Star.png")
                    {
                        await Shell.Current.DisplayAlert("Oh no!", "It looks like " + movieResult.Title + " already exits in your favorite list!", "ok");
                        AddToFavSource = "Star.png";
                        return;
                    }
                    if (await MovieService.AddMovieToFavoritesList(Result))
                    {
                        AddToFavSource = "Star.png";
                        AddToFavLayoutIsVisible = false;
                        QuitFromFavLayoutIsVisible = true;

                        OnMovieAddedRemovedUpdateUIElements?.Invoke(this, null);
                        await Shell.Current.DisplayAlert("Added Successfully", "The movie " + Result.Title + " was added to your favorite list!", "ok");
                    }
                }
            });
        }

        AsyncCommand removeFromFavListCommand;
        public AsyncCommand RemoveFromFavListCommand
        {
            get => removeFromFavListCommand ??= new(async () =>
            {
                if (await Shell.Current.DisplayAlert("Suggestion", "Would you like to delete this movie from your favorites list?", "Yes", "No"))
                {
                    if(await MovieService.RemoveMovieFromFavoriteList(Result))
                    {
                        OnMovieAddedRemovedUpdateUIElements?.Invoke(this, null);

                        AddToFavSource = "StarEmpty.png";
                        AddToFavLayoutIsVisible = true;
                        QuitFromFavLayoutIsVisible = false;

                        await Shell.Current.DisplayAlert("Deleted Successfully", "The movie " + Result.Title + " was deleted from your favorite list!", "ok");
                    }
                }
            });
        }

        AsyncCommand playTrailerCommand;
        public AsyncCommand PlayTrailerCommand
        {
            get => playTrailerCommand ??= new(async () =>
            {
                var video = await Locator.Current.GetService<ApiClient>().GetMovieVideosAsync(Result.Id);

                if (video.Results.Count() > 0)
                {
                    await Launcher.CanOpenAsync(new Uri("vnd.youtube://watch/" + video.Results.Where(v => v.Type == "Trailer").FirstOrDefault().Key));
                }
                else
                {
                    DependencyService.Get<IToast>().LongAlert("No trailers for this movie/serie found");
                }
            });
        }

        public MovieDetailsPageViewModel()
        {
            SetUpProps();
        }

        private void SetUpProps()
        {
            MovieService = Locator.Current.GetService<IMovieService>();
            Result = ResultSingleton.GetInstance();
            RealmDB = RealmDBSingleton.Current;
        }

        public async Task IsPresentInFavList()
        {
            try
            {
                Result = ResultSingleton.GetInstance();

                var movieExists = RealmDB.Find<Result>(Result.Id);

                if (movieExists != null && movieExists.FavoriteMovie == "Star.png")
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        AddToFavSource = "Star.png";

                        AddToFavLayoutIsVisible = false;

                        QuitFromFavLayoutIsVisible = true;
                    });
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        AddToFavLayoutIsVisible = true;

                        QuitFromFavLayoutIsVisible = false;
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.InnerException}");
            }
        }
    }
}
