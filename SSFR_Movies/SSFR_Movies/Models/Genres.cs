using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSFR_Movies.Models
{
    /// <summary>
    /// The Movie genre
    /// </summary>
    public partial class Genres
    {
        [JsonProperty("genres")]
        public Genre[] GenresGenres { get; set; }
    }

    public partial class Genre
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
