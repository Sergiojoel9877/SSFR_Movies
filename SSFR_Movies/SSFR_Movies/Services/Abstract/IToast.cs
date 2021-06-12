using Xamarin.Forms.Internals;

namespace SSFR_Movies.Services.Abstract
{
    [Preserve(AllMembers = true)]
    public interface IToast
    {
        void LongAlert(string msg);
        void ShortAlert(string msg);
    }
}
