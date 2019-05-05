using Newtonsoft.Json;
using Realms;

namespace SSFR_Movies.Models
{
    /// <summary>
    /// The BaseEntity.. all models inherit from it.
    /// </summary>

    public class BaseEntity : RealmObject
    {
        [JsonProperty("id")]
        public long Id { get; set; }
    }
}
