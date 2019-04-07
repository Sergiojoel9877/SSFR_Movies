using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Xamarin.Forms;
using Android.Gms.Ads;
using Android.Content;
using SSFR_Movies.Services;
using FFImageLoading;
using Refractored.XamForms.PullToRefresh.Droid;
using Android.Widget;

namespace SSFR_Movies.Droid
{
    [Android.Runtime.Preserve(AllMembers = true)]
    [Activity(Label = "SSFR_Movies", Icon = "@mipmap/icon", /*Theme = "@style/MainTheme",*/ Theme = "@style/Theme.Splash", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        readonly Lazy<App> LazyApp = new Lazy<App>(() => new App());

        protected override void OnCreate(Bundle bundle)
        {
         
            MainApplication.activity = this;

            if (Intent.GetBooleanExtra("crash", false))
            {
                Toast.MakeText(this, "App restarted after an unexpected crash, don't worry :)", ToastLength.Short).Show();
            }

            base.Window.RequestFeature(Android.Views.WindowFeatures.ActionBar);

            base.SetTheme(Resource.Style.MainTheme);

            TabLayoutResource = Resource.Layout.Tabbar;

            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            PullToRefreshLayoutRenderer.Init();

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(false);

            MobileAds.Initialize(ApplicationContext, "ca-app-pub-7678114811413714~8329396213");
            
            Forms.SetFlags(new[] { "CollectionView_Experimental", "Shell_Experimental", "Visual_Experimental", "FastRenderers_Experimental" });

            global::Xamarin.Forms.Forms.Init(this, bundle);

            XF.Material.Droid.Material.Init(this, bundle);

            Android.Glide.Forms.Init();

            LoadApplication(LazyApp.Value);
        }

        static bool _flag = false;
        static MainActivity()
        {
            if (_flag)
            {
                var ignore = new Refractored.XamForms.PullToRefresh.Droid.PullToRefreshLayoutRenderer();
            }
        }

        public async override void OnTrimMemory([GeneratedEnum] TrimMemory level)
        {
            FFImageLoading.ImageService.Instance.InvalidateMemoryCache();

            await FFImageLoading.ImageService.Instance.InvalidateDiskCacheAsync();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

            base.OnTrimMemory(level);
        }

        public async override void OnLowMemory()
        {
            FFImageLoading.ImageService.Instance.InvalidateMemoryCache();

            await FFImageLoading.ImageService.Instance.InvalidateDiskCacheAsync();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

            base.OnLowMemory();
        }
    }
}

