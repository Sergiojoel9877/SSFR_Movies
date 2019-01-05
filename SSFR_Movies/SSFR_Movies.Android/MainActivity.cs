using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Xamarin.Forms;
using Android.Gms.Ads;
using Android.Content;

namespace SSFR_Movies.Droid
{
    [Android.Runtime.Preserve(AllMembers = true)]
    [Activity(Label = "SSFR_Movies", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;

            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Forms.SetFlags(new[] { "CollectionView_Experimental", "Shell_Experimental", "Visual_Experimental", "FastRenderers_Experimental" });

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);
            
            MobileAds.Initialize(ApplicationContext, "ca-app-pub-7678114811413714~8329396213");

            global::Xamarin.Forms.Forms.Init(this, bundle);

            Android.Glide.Forms.Init();

            LoadApplication(new App());
        }

        public override void OnTrimMemory([GeneratedEnum] TrimMemory level)
        {
            FFImageLoading.ImageService.Instance.InvalidateMemoryCache();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);

            base.OnTrimMemory(level);
        }

        public override void OnLowMemory()
        {
            FFImageLoading.ImageService.Instance.InvalidateMemoryCache();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);

            base.OnLowMemory();
        }
        
    }
}

