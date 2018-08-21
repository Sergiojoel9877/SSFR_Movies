using System;
using System.Collections.Generic;
using System.Text;

namespace GBH_Movies_Test.Services
{
    public interface IToast
    {
        void LongAlert(string msg);
        void ShortAlert(string msg);
    }
}
