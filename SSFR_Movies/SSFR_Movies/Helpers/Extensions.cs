using CommonServiceLocator;
using SSFR_Movies.Data;
using SSFR_Movies.Models;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSFR_Movies.Helpers
{
    public static class Extensions
    {
        //Verify the length of the incoming string
        //to assign its value to the initial range of the animation.........
        public async static Task SetAnimation(this Label lbl)
        {
            await Task.Yield();

            Device.BeginInvokeOnMainThread(() =>
            {
                var right = new Animation(d => lbl.TranslationX = d, 350, -500);

                right.Commit(lbl, "Animation", 300, 7500, Easing.Linear, (d, b) =>
                {
                    lbl.TranslationX = 350;
                }, () => true);
            });
        }

        public static async Task IsPresentInFavList(this Result m, Image pin2FavList, long Id)
        {
            await Task.Yield();

            bool movieExists = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().EntityExits((int)Id);

            if (movieExists)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    pin2FavList.Source = "Star.png";
                });
            }
        }
    }
}
