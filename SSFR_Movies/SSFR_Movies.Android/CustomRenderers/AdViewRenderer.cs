using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SSFR_Movies.Droid.CustomRenderers;
using SSFR_Movies.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AdMobView), typeof(AdViewRenderer))]
namespace SSFR_Movies.Droid.CustomRenderers
{
    public class AdViewRenderer : ViewRenderer<AdMobView, AdView>
    {
        public AdViewRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<AdMobView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null && Control == null)
            {
                try
                {
                    SetNativeControl(CreateView());
                }
                catch (DeadObjectException e1)
                {
                    
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(AdView.AdUnitId))
            {
                Control.AdUnitId = Element.AdUnitId;
            }
        }

        private AdView CreateView()
        {
            try
            {
                var adView = new AdView(Context)
                {
                    AdSize = AdSize.Banner,
                    AdUnitId = Element.AdUnitId
                };

                adView.LayoutParameters = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);


                adView.LoadAd(new AdRequest.Builder().Build());

                return adView;
            }
            catch (DeadObjectException e)
            {
                return null;
            }
        }
    }
}