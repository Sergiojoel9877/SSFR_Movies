using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using SSFR_Movies.Services;

namespace SSFR_Movies.Droid
{
    [Android.Runtime.Preserve(AllMembers = true)]
    [Activity(Theme="@style/Theme.Splash", NoHistory = true, MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : AppCompatActivity
    {
       
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            MainApplication.activity = this;

            if (Intent.GetBooleanExtra("crash", false))
            {
                Toast.MakeText(this, "App restarted after an unexpected crash, don't worry :)", ToastLength.Short).Show();
            }

            this.StartActivity(typeof(MainActivity));
        }

    }
}