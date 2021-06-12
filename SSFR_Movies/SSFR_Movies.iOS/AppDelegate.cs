using System;
using Foundation;
using SFR_Movies.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace SSFR_Movies.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        //readonly Lazy<App> LazyApp = new Lazy<App>(() => new App());
        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method

            Rg.Plugins.Popup.Popup.Init();

            Xamarin.Forms.Forms.SetFlags("SwipeView_Experimental");

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();

            PullToRefreshLayoutRenderer.Init();

            Xamarin.Forms.Forms.Init();
            FormsMaterial.Init();
            FFImageLoading.Forms.Platform.CachedImageRenderer.InitImageSourceHandler();
            XF.Material.iOS.Material.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(application, launchOptions);
        }
    }
}

