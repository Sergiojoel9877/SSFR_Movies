using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SSFR_Movies.Helpers;
using SSFR_Movies.Models;
using SSFR_Movies.Services.Abstract;
using Xamarin.Essentials;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;
using XF.Material.Forms.UI.Dialogs.Configurations;

namespace SSFR_Movies.Services
{
    public class MovieService : IMovieService
    {
        public event EventHandler<MovieEventArgs> OnMovieAdded;
        public event EventHandler<MovieEventArgs> OnMovieRemoved;

        Realms.Realm RealmDB { get; } = RealmDBSingleton.Current;

        readonly MaterialSnackbarConfiguration Conf = new()
        {
            TintColor = Color.FromHex("#0066cc"),
            BackgroundColor = Color.FromHex("#272B2E")
        };

        public async Task<bool> AddMovieToFavoritesList(Result movie)
        {

            //Verify if internet connection is available
            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    MaterialDialog.Instance.SnackbarAsync("No internet Connection", "Dismiss", MaterialSnackbar.DurationIndefinite, Conf);
                    return false;
                });
                return await Task.FromResult(false);
            }

            try
            {
                var movieExists = RealmDB.Find<Result>(movie.Id);

                if (movieExists != null && movieExists.FavoriteMovie == "Star.png")
                {

                    Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                    {
                        MaterialDialog.Instance.SnackbarAsync("Oh no It looks like " + movie.Title + " already exits in your favorite list!", "Dismiss", MaterialSnackbar.DurationIndefinite, Conf);
                        return false;
                    });

                    return await Task.FromResult(false);
                }

                RealmDB.Write(() =>
                {
                    movie.FavoriteMovie = "Star.png";

                    RealmDB.Add(movie, true);
                });

                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    MaterialDialog.Instance.SnackbarAsync("Added Successfully, The movie " + movie.Title + " was added to your favorite list!", "Dismiss", MaterialSnackbar.DurationShort, Conf);
                    return false;
                });

                OnMovieAdded?.Invoke(this, new MovieEventArgs { Result = movie });

                return await Task.FromResult(true);
            }
            catch (Exception e15)
            {
                Debug.WriteLine("Error: " + e15.InnerException);
                return await Task.FromResult(false);
            }
        }

        public async Task<bool> RemoveMovieFromFavoriteList(Result result)
        {
            //Verify if internet connection is available
            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");
                    return false;
                });
                return await Task.FromResult(false);
            }

            try
            {
                var deleteMovie = RealmDB.Find<Result>(result.Id);

                if (deleteMovie != null)
                {
                    RealmDB.Write(() =>
                    {
                        result.FavoriteMovie = "StarEmpty.png";

                        RealmDB.Add(result, true);
                    });

                    OnMovieRemoved?.Invoke(this, new MovieEventArgs { Result = result });

                    return await Task.FromResult(true);
                }
                return await Task.FromResult(false);
            }
            catch (Exception e15)
            {
                Debug.WriteLine("Error: " + e15.InnerException);
                return await Task.FromResult(false);
            }
        }
    }
}
