using SSFR_Movies.Models;

namespace SSFR_Movies.Helpers
{
    public class DBRepoSingleton<T> where T : BaseEntity, new()
    {
        DBRepoSingleton() { }
        class DBRepoSingletonCreator
        {
            static DBRepoSingletonCreator() { }
            internal static readonly T instance = new T();
        }

        public static T UniqueInstance
        {
            get { return DBRepoSingletonCreator.instance; }
        }
    }
}
