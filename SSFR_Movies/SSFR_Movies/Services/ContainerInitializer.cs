using CommonServiceLocator;
using Microsoft.EntityFrameworkCore;
using MonkeyCache.FileStore;
using Plugin.Connectivity;
using SSFR_Movies.Data;
using SSFR_Movies.ViewModels;
using System;
using Unity;
using Unity.ServiceLocation;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace SSFR_Movies.Services
{
    /// <summary>
    /// Constainer to implement IoC
    /// </summary>
    [Preserve(AllMembers = true)]
    public class ContainerInitializer
    {
      
        public static void Initialize()
        {
            
            //Sets the barrel cache ID.. with out it, the Barrel cannot work
            Barrel.ApplicationId = "SSFR_Movies";

            var container = new Lazy<UnityContainer>(() => new UnityContainer());

            var serviceLocator = new UnityServiceLocator(container.Value);

            ServiceLocator.SetLocatorProvider(() => serviceLocator);
            
            container.Value.RegisterInstance(typeof(Lazy<ApiClient>));
            container.Value.RegisterInstance(typeof(Lazy<AllMoviesPageViewModel>));
            container.Value.RegisterInstance(typeof(Lazy<FavoriteMoviesPageViewModel>));
            //container.Value.RegisterInstance(typeof(DatabaseContext<>));
            //container.Value.RegisterInstance(typeof(DBRepository<>));
            //container.Value.RegisterType(typeof(DatabaseContext<>));
            //container.Value.RegisterType(typeof(DbContextOptionsBuilder));

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
        }
    }
}
