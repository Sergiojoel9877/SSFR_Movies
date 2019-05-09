//using SSFR_Movies.Data;
using SSFR_Movies.Models;
using System;
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
        public static async Task SetAnimation(this Label lbl)
        {
            await Task.Yield();

            if (MainThread.IsMainThread)
            {
                var right = new Animation(d => lbl.TranslationX = d, 350, -500);

                right.Commit(lbl, "Animation", 300, 7500, Easing.Linear, (d, b) =>
                {
                    lbl.TranslationX = 350;
                }, () => true);
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
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
}
