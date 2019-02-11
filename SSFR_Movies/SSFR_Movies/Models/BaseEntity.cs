using Newtonsoft.Json;
using Realms;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Internals;

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
