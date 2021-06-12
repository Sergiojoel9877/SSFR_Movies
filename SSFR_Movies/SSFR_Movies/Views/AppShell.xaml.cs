using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Splat;
using SSFR_Movies.Services;
using SSFR_Movies.Services.Abstract;
using SSFR_Movies.ViewModels;
using Xamarin.Forms;

namespace SSFR_Movies.Views
{

    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            RegisterRoutes();

            new Lazy<ContainerInitializer>(() => new ContainerInitializer()).Value.Initialize();
        }

        void RegisterRoutes()
        {
            Routing.RegisterRoute("Search", typeof(SearchPage));
            Routing.RegisterRoute("MovieDetails", typeof(MovieDetailsPage));
        }

        protected override bool OnBackButtonPressed()
        {
            var c = DependencyService.Get<ICloseBackPress>();

            if (c != null)
            {
                c.Close();
                base.OnBackButtonPressed();
            }

            return true;
        }
    }
}