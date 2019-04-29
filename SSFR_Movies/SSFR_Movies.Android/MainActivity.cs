﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Xamarin.Forms;
using Android.Gms.Ads;
using Android.Content;
using Android.Widget;
using SSFR_Movies.Droid.CustomRenderers;

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

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);

            MobileAds.Initialize(ApplicationContext, "ca-app-pub-7678114811413714~8329396213");
            
            Forms.SetFlags(new[] { "CollectionView_Experimental", "Shell_Experimental", "FastRenderers_Experimental" });

            global::Xamarin.Forms.Forms.Init(this, bundle);

            PullToRefreshLayoutRenderer.Init();

            //FFImageLoading.ImageSourceHandler();

            global::Xamarin.Forms.FormsMaterial.Init(this, bundle);

            XF.Material.Droid.Material.Init(this, bundle);

            Android.Glide.Forms.Init();

            LoadApplication(LazyApp.Value);

        }
#pragma warning disable 0219, 0649
        static MainActivity()
        {
            bool flasg = false;

            if (flasg)
            {
                var dummy = typeof(FFImageLoading.Forms.Platform.CachedImageFastRenderer);
                //var dummy1 = typeof(PullToRefreshLayoutRenderer);
            }
        }
#pragma warning restore

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

