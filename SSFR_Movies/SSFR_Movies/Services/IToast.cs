using System;
using System.Collections.Generic;
using System.Text;

namespace SSFR_Movies.Services
{
    public interface IToast
    {
        void LongAlert(string msg);
        void ShortAlert(string msg);
    }
}
