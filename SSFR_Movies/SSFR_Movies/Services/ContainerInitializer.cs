using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using SSFR_Movies.ViewModels;
using SSFR_Movies.Data;
using SSFR_Movies.Services;
using CommonServiceLocator;
using Unity.ServiceLocation;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using MonkeyCache.FileStore;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Xamarin.Forms;

namespace SSFR_Movies.Services
{
    /// <summary>
    /// Constainer to implement IoC
    /// </summary>
    public class ContainerInitializer
    {
      
        public static void Initialize()
        {
            //Verify if internet connection is available
            if (!CrossConnectivity.Current.IsConnected)
            {
                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");
                    return false;
                });
                return;
            }

            //Sets the barrel cache ID.. with out it, the Barrel cannot work
            Barrel.ApplicationId = "SSFR_Movies";

            var container = new UnityContainer();

            var serviceLocator = new UnityServiceLocator(container);

            ServiceLocator.SetLocatorProvider(() => serviceLocator);

            //Lazy<ApiClient> apiClient = new Lazy<ApiClient>();
            
            //Lazy<AllMoviesPageViewModel> allMoviesPageViewModel = new Lazy<AllMoviesPageViewModel>();

            //Lazy<FavoriteMoviesPageViewModel> favoritesMoviesPageViewModel = new Lazy<FavoriteMoviesPageViewModel>();

            //Lazy<DbContextOptionsBuilder> dbContextOptionsBuilder = new Lazy<DbContextOptionsBuilder>();

            container.RegisterInstance(typeof(ApiClient));
            container.RegisterInstance(typeof(AllMoviesPageViewModel));
            container.RegisterInstance(typeof(FavoriteMoviesPageViewModel));
            container.RegisterInstance(typeof(DBRepository<>));
            container.RegisterInstance(typeof(DatabaseContext<>));
            container.RegisterType(typeof(DatabaseContext<>));
            container.RegisterType(typeof(DbContextOptionsBuilder));
            
        }
      
    }
}
