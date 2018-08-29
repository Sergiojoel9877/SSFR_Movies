using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Internals;

namespace SSFR_Movies.Models
{
    /// <summary>
    /// The BaseEntity.. all models inherit from it.
    /// </summary>
  
    public class BaseEntity
    {
        [JsonProperty("id")]
        public long Id { get; set; }
    }
}
