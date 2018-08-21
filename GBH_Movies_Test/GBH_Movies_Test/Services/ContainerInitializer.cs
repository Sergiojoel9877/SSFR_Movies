using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using GBH_Movies_Test.ViewModels;
using GBH_Movies_Test.Data;
using GBH_Movies_Test.Services;
using CommonServiceLocator;
using Unity.ServiceLocation;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using MonkeyCache.FileStore;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Xamarin.Forms;

namespace GBH_Movies_Test.Services
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
            Barrel.ApplicationId = "GBH_MOVIES_TEST";

            var container = new UnityContainer();

            var serviceLocator = new UnityServiceLocator(container);

            ServiceLocator.SetLocatorProvider(() => serviceLocator);

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
