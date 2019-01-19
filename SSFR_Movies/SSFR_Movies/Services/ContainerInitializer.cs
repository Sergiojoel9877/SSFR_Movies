﻿using System;
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
            container.Value.RegisterInstance(typeof(DBRepository<>));
            container.Value.RegisterInstance(typeof(DatabaseContext<>));
            container.Value.RegisterType(typeof(DatabaseContext<>));
            container.Value.RegisterType(typeof(DbContextOptionsBuilder));

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
