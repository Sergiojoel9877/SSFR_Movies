using System;
using System.Collections.Generic;
using System.Text;

namespace GBH_Movies_Test.Models
{
    /// <summary>
    /// The Movie obj model
    /// </summary>
    public class Movie : BaseEntity
    {  
        public string BackDropPath { get; set; }
        public string PosterPath { get; set; }
        public List<Genre> Genres { get; set; }
        public int VoteCount { get; set; }
        public string OriginalTitle { get; set; }
        public string Overview { get; set; }
        public string ReleaseDate { get; set; }
    }
}
