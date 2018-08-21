//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace GBH_Movies_Test.Models
//{
//    /// <summary>
//    /// The Movie obj model
//    /// </summary>
//    public class Movie : BaseEntity
//    {  
//        //public string BackDropPath { get; set; }
//        //public string PosterPath { get; set; }
//        //public List<Genre> Genres { get; set; }
//        //public int VoteCount { get; set; }
//        //public string OriginalTitle { get; set; }
//        //public string Overview { get; set; }
//        //public string ReleaseDate { get; set; }

//    }
//}
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var movie = Movie.FromJson(jsonString);
    using System;
    using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

namespace GBH_Movies_Test.Models
{

    public class Movie : BaseEntity
    {
        [JsonProperty("page")]
        public long Page { get; set; }

        [JsonProperty("total_results")]
        public long TotalResults { get; set; }

        [JsonProperty("total_pages")]
        public long TotalPages { get; set; }

        [JsonProperty("results")]
        public List<Result> Results { get; set; }
       
    }

    public class Result : BaseEntity
    {
        [JsonProperty("vote_count")]
        public long VoteCount { get; set; }

        [JsonProperty("video")]
        public bool Video { get; set; }

        [JsonProperty("vote_average")]
        public double VoteAverage { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("popularity")]
        public double Popularity { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("original_language")]
        public OriginalLanguage OriginalLanguage { get; set; }

        [JsonProperty("original_title")]
        public string OriginalTitle { get; set; }

        [NotMapped]
        [JsonProperty("genre_ids")]
        public int[] GenreIds { get; set; }

        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonProperty("adult")]
        public bool Adult { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }

    }

    public enum OriginalLanguage { En };
}

