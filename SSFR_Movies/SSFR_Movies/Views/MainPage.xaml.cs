﻿using SSFR_Movies.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : Xamarin.Forms.TabbedPage
    {
        public MainPage ()
        {
            InitializeComponent();

            //Task.Run(new Action(InitializeIoCContainer));

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetBarItemColor(Color.FromHex("#0088FF"));

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(true);

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetBarSelectedItemColor(Color.White);

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetElevation(5.0f);

        }

        ///// <summary>
        ///// Initialize the IoC container and its Service Locator
        ///// </summary>
        //private void InitializeIoCContainer() => ContainerInitializer.Initialize();

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