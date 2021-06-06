using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace SSFR_Movies.Helpers
{
    [Preserve(AllMembers = true)]
    public static class Extensions
    {
        //Verify the length of the incoming string
        //to assign its value to the initial range of the animation.........
        //TODO: FIX LAG
        public static void SetAnimation(this Label lbl)
        {
            MainThread.BeginInvokeOnMainThread(()=>
            {
                var Go = new Animation(d => lbl.TranslationX = d, 350, -500);

                Go.Commit(lbl, "Animation", 250, 7500, Easing.Linear, (d, b) =>
                {
                    lbl.TranslationX = 350;
                }, () => true);
            });
        }
    }
}
