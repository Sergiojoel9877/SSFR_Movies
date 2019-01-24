﻿using System;

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
using Plugin.MediaManager.Forms.Android;
using Plugin.MediaManager;
using Plugin.MediaManager.MediaSession;

namespace SSFR_Movies.Droid
{
    [Android.Runtime.Preserve(AllMembers = true)]
    [Activity(Label = "SSFR_Movies", Icon = "@mipmap/icon", /*Theme = "@style/MainTheme",*/ Theme = "@style/Theme.Splash", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        Lazy<App> LazyApp = new Lazy<App>(() => new App());

        protected override void OnCreate(Bundle bundle)
        {
            ((MediaManagerImplementation)CrossMediaManager.Current).MediaSessionManager = new MediaSessionManager(Application.Context, typeof(ExoPlayerAudioService));
            var exoPlayer = new ExoPlayerAudioImplementation(((MediaManagerImplementation)CrossMediaManager.Current).MediaSessionManager);
            CrossMediaManager.Current.AudioPlayer = exoPlayer;

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

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(false);

            PullToRefreshLayoutRenderer.Init();

            VideoViewRenderer.Init();

            MobileAds.Initialize(ApplicationContext, "ca-app-pub-7678114811413714~8329396213");
            
            Forms.SetFlags(new[] { "CollectionView_Experimental", "Shell_Experimental", "Visual_Experimental", "FastRenderers_Experimental" });

            global::Xamarin.Forms.Forms.Init(this, bundle);

            Android.Glide.Forms.Init();

            LoadApplication(LazyApp.Value);
        }

        public override async void OnTrimMemory([GeneratedEnum] TrimMemory level)
        {
            FFImageLoading.ImageService.Instance.InvalidateMemoryCache();

            await FFImageLoading.ImageService.Instance.InvalidateDiskCacheAsync();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

            base.OnTrimMemory(level);
        }

        public override async void OnLowMemory()
        {
            FFImageLoading.ImageService.Instance.InvalidateMemoryCache();

            await FFImageLoading.ImageService.Instance.InvalidateDiskCacheAsync();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

            base.OnLowMemory();
        }
    }
}

