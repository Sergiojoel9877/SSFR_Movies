using Newtonsoft.Json;
using Realms;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSFR_Movies.Models
{
    /// <summary>
    /// The Movie genre
    /// </summary>
    public partial class Genres : RealmObject
    {
        [JsonProperty("genres")]
        public Genre[] GenresGenres { get; set; }
    }

    public partial class Genre : RealmObject
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
