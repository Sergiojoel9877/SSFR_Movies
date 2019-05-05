using Android.Content;
using Android.Runtime;
using SSFR_Movies.CustomRenderers;
using SSFR_Movies.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TransparentEntry), typeof(TransparentEntryRenderer))]
namespace SSFR_Movies.Droid.CustomRenderers
{
    [Preserve(AllMembers = true)]
    public class TransparentEntryRenderer : EntryRenderer
    {
        public TransparentEntryRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var element = e.NewElement as TransparentEntry;
                Control.Hint = element.Placeholder;

                Control?.SetBackgroundColor(Android.Graphics.Color.Transparent);

                // Text alignment.
                if (element.HorizontalTextAlignment == Xamarin.Forms.TextAlignment.Center)
                {
                    Control.Gravity = Android.Views.GravityFlags.CenterHorizontal;
                }
                else if (element.HorizontalTextAlignment == Xamarin.Forms.TextAlignment.Start)
                {
                    Control.Gravity = Android.Views.GravityFlags.Start;
                }
                else if (element.HorizontalTextAlignment == Xamarin.Forms.TextAlignment.End)
                {
                    Control.Gravity = Android.Views.GravityFlags.End;
                }
            }
        }
    }
}