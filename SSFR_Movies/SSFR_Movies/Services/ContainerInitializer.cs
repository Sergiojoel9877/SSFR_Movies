using Splat;
using SSFR_Movies.CustomRenderers;
using SSFR_Movies.ViewModels;
using System;
using Xamarin.Forms.Internals;
using AsyncAwaitBestPractices;
using Xamarin.Forms;
using SSFR_Movies.Services.Abstract;

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
            Locator.CurrentMutable.RegisterLazySingleton(() => new AllMoviesPageViewModel(), typeof(AllMoviesPageViewModel));
            Locator.CurrentMutable.Register(() => new FavoriteMoviesPageViewModel(), typeof(FavoriteMoviesPageViewModel));
            Locator.CurrentMutable.Register(() => new MovieDetailsPageViewModel(), typeof(MovieDetailsPageViewModel));
            Locator.CurrentMutable.RegisterLazySingleton(()=> new MovieService(), typeof(IMovieService));
        }
#pragma warning disable 0219, 0649
        static readonly bool falseflag = false;
        static ContainerInitializer()
        {
            if (falseflag)
            {
                var ignore = new Lazy<ApiClient>(() => new ApiClient());
                var ignore2 = new Lazy<AllMoviesPageViewModel>(() => new AllMoviesPageViewModel());
                var ignore3 = new Lazy<FavoriteMoviesPageViewModel>(() => new FavoriteMoviesPageViewModel());
                var ignore5 = typeof(Rg.Plugins.Popup.Animations.Base.FadeBackgroundAnimation);
                var ignore7 = typeof(Device);
                var ignore8 = typeof(MovieService);
                var ignore9 = typeof(IMovieService);
            }
        }
#pragma warning restore 0219, 0649
    }
}
