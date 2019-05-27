using Splat;
using SSFR_Movies.CustomRenderers;
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
            Locator.CurrentMutable.RegisterLazySingleton(() => new AllMoviesPageViewModel(), typeof(AllMoviesPageViewModel));
            Locator.CurrentMutable.RegisterLazySingleton(() => new FavoriteMoviesPageViewModel(), typeof(FavoriteMoviesPageViewModel));
            Locator.CurrentMutable.Register(() => new PullToRefreshLayout(), typeof(PullToRefreshLayout));
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
            }
        }
#pragma warning restore 0219, 0649
    }
}
