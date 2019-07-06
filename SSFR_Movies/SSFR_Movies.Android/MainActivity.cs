using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AsyncAwaitBestPractices;

//using FFImageLoading;
using SSFR_Movies.Droid.CustomRenderers;
using SSFR_Movies.Droid.Services;
using SSFR_Movies.Helpers;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SSFR_Movies.Droid
{
    [Android.Runtime.Preserve(AllMembers = true)]
    [Activity(Label = "SSFR_Movies", Icon = "@mipmap/icon", Theme = "@style/Theme.Splash", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleInstance)]
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

            Forms.SetFlags(new[] { "CollectionView_Experimental" });

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);

            //var config = new FFImageLoading.Config.Configuration()
            //{
            //    VerboseLogging = false,
            //    VerbosePerformanceLogging = false,
            //    VerboseMemoryCacheLogging = false,
            //    VerboseLoadingCancelledLogging = false,
            //    Logger = new CustomLogger(),
            //};
            //ImageService.Instance.Initialize(config);

            MobileAds.Initialize(ApplicationContext, "ca-app-pub-7678114811413714~8329396213");

            Rg.Plugins.Popup.Popup.Init(this, bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            FFImageLoading.Forms.Platform.CachedImageRenderer.InitImageViewHandler();

            Android.Glide.Forms.Init(this);

            PullToRefreshLayoutRenderer.Init();

            global::Xamarin.Forms.FormsMaterial.Init(this, bundle);

            XF.Material.Droid.Material.Init(this, bundle);

            LoadApplication(LazyApp.Value);
        }

#pragma warning disable 0219, 0649
        static MainActivity()
        {
            bool flasg = false;

            if (flasg)
            {
                var dummy = typeof(FFImageLoading.Forms.Platform.CachedImageFastRenderer);
            }
        }
#pragma warning restore

        public override async void OnTrimMemory([GeneratedEnum] TrimMemory level)
        {
            new SynchronizationContextRemover();

            FFImageLoading.ImageService.Instance.InvalidateMemoryCache();

            await FFImageLoading.ImageService.Instance.InvalidateDiskCacheAsync();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);
            
            base.OnTrimMemory(level);
        }

        public override async void OnLowMemory()
        {
            new SynchronizationContextRemover();

            FFImageLoading.ImageService.Instance.InvalidateMemoryCache();

            await FFImageLoading.ImageService.Instance.InvalidateDiskCacheAsync();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);

            base.OnLowMemory();
        }

    }
}

