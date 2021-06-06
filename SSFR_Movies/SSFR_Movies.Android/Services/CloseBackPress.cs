
using Android.App;
using Android.Content;
using Android.Views;
using SSFR_Movies.Droid.Services;
using SSFR_Movies.Services.Abstract;
using Xamarin.Forms;

[assembly: Dependency(typeof(CloseBackPress))]
namespace SSFR_Movies.Droid.Services
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class CloseBackPress : Xamarin.Forms.Platform.Android.FormsAppCompatActivity, ICloseBackPress
    {

#pragma warning disable CS0618
        readonly Activity activity = (MainActivity)Forms.Context;
#pragma warning restore CS068

        public void Close()
        {
            OnBackPressed();
        }

        public override void OnBackPressed()
        {

            ShowAlert("Exit", "Are you sure that you wanna exit?");

        }

        AlertDialog.Builder Ab;

        public void ShowAlert(string title, string msg)
        {
            AB(title, msg, "Ok");
        }

        public void AB(string title, string msg, string yes)
        {

            Ab = new AlertDialog.Builder(activity);

            Ab.SetTitle(title).SetMessage(msg).SetPositiveButton(yes, OnClick);

            var create = Ab.Create();

            try
            {
                create.Show();
            }
            catch (WindowManagerBadTokenException)
            {
                activity.FinishAffinity();
            }

        }

        public void OnClick(object dialog, DialogClickEventArgs e)
        {
            activity.FinishAffinity();
        }
    }
}