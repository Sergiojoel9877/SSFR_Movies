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
using Xamarin.Forms.Internals;

namespace SSFR_Movies.Services
{
    /// <summary>
    /// Constainer to implement IoC
    /// </summary>
  
    public class ContainerInitializer
    {
      
        public ContainerInitializer()
        {
                
        }

      
        public static void Initialize()
        {
         
      
            var container = new UnityContainer();

            var serviceLocator = new UnityServiceLocator(container);

            ServiceLocator.SetLocatorProvider(() => serviceLocator);
            
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
