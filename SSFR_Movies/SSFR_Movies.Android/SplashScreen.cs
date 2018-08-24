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

            MainApplication.activity = this;

            if (Intent.GetBooleanExtra("crash", false))
            {
                Toast.MakeText(this, "App restarted after an unexpected crash, don't worry :)", ToastLength.Short).Show();
            }

            this.StartActivity(typeof(MainActivity));

        }

    }
}