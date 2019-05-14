﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Widget;
//using FFImageLoading;
using SSFR_Movies.Droid.CustomRenderers;
using System;
using Xamarin.Forms;

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

            Forms.SetFlags(new[] { "CollectionView_Experimental", "Shell_Experimental", "FastRenderers_Experimental"});

            //FFImageLoading.Forms.Platform.CachedImageRenderer.Init(false);

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

            //FFImageLoading.Forms.Platform.CachedImageRenderer.InitImageViewHandler();

            Android.Glide.Forms.Init();

            //SSFR_Movies.Droid.Effects.TouchEffectPlatform.Init();

            PullToRefreshLayoutRenderer.Init();

            //FFImageLoading.ImageSourceHandler();

            global::Xamarin.Forms.FormsMaterial.Init(this, bundle);

            XF.Material.Droid.Material.Init(this, bundle);

            LoadApplication(LazyApp.Value);

        }

        //public class CustomLogger : FFImageLoading.Helpers.IMiniLogger
        //{
        //    public void Debug(string message)
        //    {
        //        Console.WriteLine(message);
        //    }

        //    public void Error(string errorMessage)
        //    {
        //        Console.WriteLine(errorMessage);
        //    }%

        //    public void Error(string errorMessage, Exception ex)
        //    {
        //        Error(errorMessage + System.Environment.NewLine + ex.ToString());
        //    }
        //}
#pragma warning disable 0219, 0649
        static MainActivity()
        {
            bool flasg = false;

            if (flasg)
            {
                //var dummy = typeof(FFImageLoading.Forms.Platform.CachedImageFastRenderer);
                //var dummy1 = typeof(PullToRefreshLayoutRenderer);
                var dummy1 = typeof(SSFR_Movies.Droid.Effects.TouchEffectPlatform);
            }
        }
#pragma warning restore

        public override void OnTrimMemory([GeneratedEnum] TrimMemory level)
        {
            //FFImageLoading.ImageService.Instance.InvalidateMemoryCache();

            //await FFImageLoading.ImageService.Instance.InvalidateDiskCacheAsync();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            
            base.OnTrimMemory(level);
        }

        public override void OnLowMemory()
        {
            //FFImageLoading.ImageService.Instance.InvalidateMemoryCache();

            //await FFImageLoading.ImageService.Instance.InvalidateDiskCacheAsync();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

            base.OnLowMemory();
        }
    }
}

