using MonkeyCache.FileStore;
using Plugin.Connectivity;
using SSFR_Movies.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Splat;

namespace SSFR_Movies.Services
{
    /// <summary>
    /// Constainer to implement IoC
    /// </summary>
    [Preserve(AllMembers = true)]
    public class ContainerInitializer
    {
        public void Initialize()
        {
            Locator.CurrentMutable.RegisterLazySingleton(() => new ApiClient(), typeof(ApiClient));
            #region Fixes Duplication
            //Fixes a weird duplication between the SearchPage and AllMoviesPage [cause they share the Same ViewModel => AllMoviesPageViewModel]
            Locator.CurrentMutable.Register(() => new AllMoviesPageViewModel(), typeof(AllMoviesPageViewModel));
            #endregion
            Locator.CurrentMutable.RegisterLazySingleton(() => new FavoriteMoviesPageViewModel(), typeof(FavoriteMoviesPageViewModel));
        }
#pragma warning disable 0219, 0649
        static bool falseflag = false;
        static ContainerInitializer()
        {
            if (falseflag)
            {
                var ignore = new Lazy<ApiClient>(() => new ApiClient());
                var ignore2 = new Lazy<AllMoviesPageViewModel>(() => new AllMoviesPageViewModel());
                var ignore3 = new Lazy<FavoriteMoviesPageViewModel>(() => new FavoriteMoviesPageViewModel());
            }
        }
#pragma warning restore 0219, 0649
    }


}
