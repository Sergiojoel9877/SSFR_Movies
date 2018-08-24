using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSFR_Movies.Models
{
    public class MovieVideo
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("results")]
        public VideoResult[] Results { get; set; }
    }

    public class VideoResult
    {
        [JsonProperty("id")]
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
   