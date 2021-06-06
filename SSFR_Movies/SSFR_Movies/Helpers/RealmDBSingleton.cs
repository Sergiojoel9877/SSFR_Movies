using System;
using Realms;

namespace SSFR_Movies.Helpers
{
    public sealed class RealmDBSingleton
    {
        static readonly Realm Instance = Realm.GetInstance();
        public static Realm Current
        {
            get => Instance;
        }
    }
}
