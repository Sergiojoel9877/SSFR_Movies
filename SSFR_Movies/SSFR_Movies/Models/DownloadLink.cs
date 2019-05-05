﻿using Newtonsoft.Json;
using System;

namespace SSFR_Movies.Models
{
    public class DownloadLink
    {
        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("result")]
        public Result Result { get; set; }
    }

    public class ResultDW
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("sha1")]
        public string Sha1 { get; set; }

        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        [JsonProperty("upload_at")]
        public DateTimeOffset UploadAt { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }

}
