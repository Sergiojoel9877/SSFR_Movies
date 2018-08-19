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

namespace GBH_Movies_Test.Services
{
    /// <summary>
    /// Constainer to implement IoC
    /// </summary>
    public sealed class ContainerInitializer
    {
        public static void Initialize()
        {
            var container = new UnityContainer();

            var serviceLocator = new UnityServiceLocator(container);

            ServiceLocator.SetLocatorProvider(() => serviceLocator);

            container.RegisterInstance(typeof(AllMoviesPageViewModel));
            container.RegisterInstance(typeof(FavoriteMoviesPageViewModel));
            container.RegisterInstance(typeof(DBRepository<>));
            container.RegisterInstance(typeof(DatabaseContext<>));
            container.RegisterType(typeof(DatabaseContext<>));
            container.RegisterType(typeof(DbContextOptionsBuilder));

          
        }
    }
}
