using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Internals;

namespace SSFR_Movies.Services
{
    [Preserve(AllMembers = true)]
    public interface IToast
    {
        void LongAlert(string msg);
        void ShortAlert(string msg);
    }
}
