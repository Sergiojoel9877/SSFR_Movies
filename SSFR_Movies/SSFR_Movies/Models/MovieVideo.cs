using Newtonsoft.Json;
using Realms;
using System.Collections;
using System.Collections.Generic;

namespace SSFR_Movies.Models
{
    public class MovieVideo : RealmObject
    {
        [JsonProperty("id")]
        [PrimaryKey]
        public int Id { get; set; }

        [JsonProperty("results")]
        public IList<VideoResult> Results { get; }
    }

    public class VideoResult : RealmObject
    {
        [JsonProperty("id")]
        [PrimaryKey]
        public string Id { get; set; }

        [JsonProperty("iso_639_1")]
        public string Iso639_1 { get; set; }

        [JsonProperty("iso_3166_1")]
        public string Iso3166_1 { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("site")]
        public string Site { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
   