using System;
using SSFR_Movies.Models;

namespace SSFR_Movies.Helpers
{
    public class MovieEventArgs : EventArgs
    {
        public Result Result { get; set; }
    }
}
