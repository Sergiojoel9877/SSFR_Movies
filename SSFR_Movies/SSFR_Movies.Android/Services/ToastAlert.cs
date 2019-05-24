using Android.App;
using Android.Widget;
using SSFR_Movies.Services;
using System.Security;

[assembly: Xamarin.Forms.Dependency(typeof(SSFR_Movies.Droid.ToastAlert))]
namespace SSFR_Movies.Droid
{
    [Android.Runtime.Preserve(AllMembers = true)]
    [SecurityCritical]
    public class ToastAlert : IToast
    {
        public void LongAlert(string msg)
        {
            Toast.MakeText(Application.Context, msg, ToastLength.Long).Show();
        }

        [SecurityCritical]
        public void ShortAlert(string msg)
        {
            Toast.MakeText(Application.Context, msg, ToastLength.Short).Show();
        }
    }
}