using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace SSFR_Movies.Droid
{
   
    [Activity(Label = "SSFR_Movies", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
       
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Forms.SetFlags("FastRenderers_Experimental");
                
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            Android.Glide.Forms.Init();

            LoadApplication(new App());
        }
    }
}

