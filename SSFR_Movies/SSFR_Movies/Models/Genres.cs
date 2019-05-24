using Newtonsoft.Json;
using Realms;
using System.Collections.Generic;

namespace SSFR_Movies.Models
{
    /// <summary>
    /// The Movie genre
    /// </summary>
    public partial class Genres : RealmObject
    {
        [JsonProperty("genres")]
        public IList<Genre> GenresGenres { get; }
    }

    public partial class Genre : RealmObject
    {
        [JsonProperty("id")]
        [PrimaryKey]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
