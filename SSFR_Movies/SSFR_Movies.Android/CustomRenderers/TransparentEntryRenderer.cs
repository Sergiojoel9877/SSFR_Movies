using System;
using Xamarin.Forms.Platform.Android;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using SSFR_Movies.CustomRenderers;
using SSFR_Movies.Droid.CustomRenderers;
using SSFR_Movies;

[assembly: ExportRenderer(typeof(TransparentEntry), typeof(TransparentEntryRenderer))]
namespace SSFR_Movies.Droid.CustomRenderers
{
    public class TransparentEntryRenderer : EntryRenderer
    {
        public TransparentEntryRenderer(Context context) : base(context)
        {
             
        }
        
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if(e.NewElement != null)
            {
                var element = e.NewElement as TransparentEntry;
                Control.Hint = element.Placeholder;

                Control?.SetBackgroundColor(Android.Graphics.Color.Transparent);

                // Text alignment.
                if (element.HorizontalTextAlignment == Xamarin.Forms.TextAlignment.Center)
                    Control.Gravity = Android.Views.GravityFlags.CenterHorizontal;
                else if (element.HorizontalTextAlignment == Xamarin.Forms.TextAlignment.Start)
                    Control.Gravity = Android.Views.GravityFlags.Start;
                else if (element.HorizontalTextAlignment == Xamarin.Forms.TextAlignment.End)
                    Control.Gravity = Android.Views.GravityFlags.End;

            }
        }
    }
}