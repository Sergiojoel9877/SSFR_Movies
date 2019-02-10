using System.Collections.Generic;
using Newtonsoft.Json;
using Realms;
using System.Linq;
using System;
using Newtonsoft.Json.Converters;

namespace SSFR_Movies.Models
{

    public class Movie : RealmObject
    {
        [PrimaryKey]
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("page")]
        public long Page { get; set; }

        [JsonProperty("total_results")]
        public long TotalResults { get; set; }

        [JsonProperty("total_pages")]
        public long TotalPages { get; set; }

        [JsonProperty("results")]
        public IList<Result> Results { get; }
       
    }

  
    public class Result : RealmObject
    {
        [JsonProperty("id")]
        [PrimaryKey]
        public int Id { get; set; }

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
        [JsonConverter(typeof(StringEnumConverter))]
        [Ignored]
        public OriginalLanguage OriginalLanguage { get; set; }

        [JsonProperty("original_title")]
        public string OriginalTitle { get; set; }
        
        [JsonProperty("genre_ids")]
        public IList<int> GenreIds { get; }

        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonProperty("adult")]
        public bool Adult { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }

    }
    
    public enum OriginalLanguage { En, Ja, Zh, Ko, Ta, Fr, Ru, Es, No, Hi, Hr, Ht, Hu, Hy, Ar, As, Av, Az, Ba, Be, Bg, Bh, Bm, Bn, Bo, Br, Bs, Ca, Ce, Ch, Co, Cr, Cs, Cn, Cu, Cv, Cy, Da, De, Dv, Ee, El, Eo, Et, Eu, Fa, Ff, Fi, Fj, Fl, Fo, Fy, Ga, Gd, Gl, Gn, Gu, Gv, Ha, He, Ho, Hz, Ia, Id, Ie, Ig, Ii, Ik, Io, Is, It, Iu,  Jv, Ka, Kg, Ki, Kj, Kk, Kl, Km, Kn, Kr, Ks, Ku, Kv, Kw, Ky, La, Lb, Li, Ln, Lo, Lt, Lu, Lv, Mg, Mh, Mi, Mk, Ml, Mn, Mr, Ms, Mt, My, Na, Nb, Nd, Ne, Ng, Nl, Nn, Nr, Nv, Ny, Oc, Oj, Om, Or, Os, Pa, pi, Pl, Ps, Pt, Qu, Rm, Rn, Ro, Rw, Sa, Sc, Sd, Se,Sg, Si, Sk, Sl, Sm, Sn, So, Sq, Sr, Ss, St, Su, Sv, Sw, Te, Tg, Th, Ti, Tk, Tl, Tn, To, Tr, Ts, Tt, Tw, Ty, Ug, Uk, Ur, Uz, Ve, Vi, Vo, Wa, Wo, Xh, Yi, Yo, Za, Ah, Zu};

    public class EnumeOL : RealmObject
    {
        string EnumDescription { get; set; }

        public void SaveEnum(OriginalLanguage value)
        {
            EnumDescription = value.ToString();
        }
    }
}

