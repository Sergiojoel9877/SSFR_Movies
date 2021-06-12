using SSFR_Movies.Models;
using System.Threading.Tasks;

namespace SSFR_Movies.Helpers
{
    public class ResultSingleton
    {
        static Result Result { get; set; }

        public static Result GetInstance()
        {
            return Result ??= new Result();
        }

        public static void SetInstance(Result res)
        {
            if (res == null)
                return;

            Result = res;
        }

        public static void SetIntanceToNull()
        {
            Result = null;
        }
    }
}
