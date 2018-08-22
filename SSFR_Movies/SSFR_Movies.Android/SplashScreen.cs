using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SSFR_Movies.Services;

namespace SSFR_Movies.Droid
{
    [Activity(Theme="@style/Theme.Splash", NoHistory= true, MainLauncher = true)]
    public class SplashScreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Toast.MakeText(this, "Initializing Resources, please wait :)", ToastLength.Long).Show();

            this.StartActivity(typeof(MainActivity));

            Task.Run(new Action(InitializeIoCContainer));
            
        }

        /// <summary>
        /// Initialize the IoC container and its Service Locator
        /// </summary>
        private void InitializeIoCContainer() => ContainerInitializer.Initialize();
    }
}