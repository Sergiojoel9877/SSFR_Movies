using Splat;
using SSFR_Movies.ViewModels;
using System;
using Xamarin.Forms.Internals;

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
            Locator.CurrentMutable.Register(() => new AllMoviesPageViewModel(), typeof(AllMoviesPageViewModel));
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
