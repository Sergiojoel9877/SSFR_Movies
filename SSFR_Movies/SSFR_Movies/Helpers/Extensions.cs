//using SSFR_Movies.Data;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace SSFR_Movies.Helpers
{
    [Preserve(AllMembers = true)]
    public static class Extensions
    {
        //Verify the length of the incoming string
        //to assign its value to the initial range of the animation.........
        public static async Task SetAnimation(this Label lbl)
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                var right = new Animation(d => lbl.TranslationX = d, 350, -500);

                right.Commit(lbl, "Animation", 300, 7500, Easing.Linear, (d, b) =>
                {
                    lbl.TranslationX = 350;
                }, () => true);
            });
        }
    }
}
