using SSFR_Movies.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSFR_Movies.Helpers
{
    public class ResultSingleton
    {
        static Result Result { get; set; }
        public static Result Instance()
        {
            if (Result == null)
            {
                Result = new Result();
            }

            return Result;
        }

        public static Result SetInstance(Result res)
        {
            Result = res;

            return Result;
        }
    }
}
