using SSFR_Movies.Models;
using System.Threading.Tasks;

namespace SSFR_Movies.Helpers
{
    public class ResultSingleton
    {
        static Result Result { get; set; }
        public static Task<Result> GetInstanceAsync()
        {
            var tcs = new TaskCompletionSource<Result>();

            if (Result == null)
            {
                Result = new Result();
                tcs.SetResult(Result);
            }

            tcs.SetResult(Result);
            return tcs.Task;
        }

        public static Task<object> SetInstanceAsync(Result res)
        {
            var tcs = new TaskCompletionSource<object>();

            Result = res;
            tcs.SetResult(null);

            return tcs.Task;
        }

        public static void SetIntanceToNull()
        {
            Result = null;
        }
    }
}
