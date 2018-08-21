using CommonServiceLocator;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using GBH_Movies_Test.Models;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using MonkeyCache;
using MonkeyCache.FileStore;
using GBH_Movies_Test.Helpers;

namespace GBH_Movies_Test.Services
{
    /// <summary>
    /// To get all movies and store them in cache. 
    /// </summary>
    public class ApiClient
    {
        private const string API_KEY = "766bc32f686bc7f4d8e1c4694b0376a8";

        private const string LANG = "en-US";

        public ApiClient()
        {
        }

        public async Task<bool> GetAndStoreMoviesAsync(bool include_video, string sortby = "popularity.desc", bool include_adult = false, int page = 1, int genres = 12)
        {
            Settings.NextPage = page;

            await Task.Yield();

            var valid = genres <= 12 ? true : false;

            string _genres = valid == true ? Convert.ToString(genres) : null;

            var requestUri = $"/3/discover/movie?api_key={API_KEY}&language={LANG}&sort_by={sortby}&include_adult={include_adult.ToString().ToLower()}&include_video={include_video.ToString().ToLower()}&page={page}&with_genres={_genres}";
            
            var m = await App.httpClient.GetAsync(requestUri);

            var results = await m.Content.ReadAsStringAsync();

            return StoreInCache(results);
            
        }

        //CREATE GETMOVIESBYGENRE


        public async Task<bool> GetAndStoreMovieGneresAsync()
        {
            
            await Task.Yield();

            var requestUri = $"/genre/movie/list?api_key={API_KEY}&language={LANG}";

            var m = await App.httpClient.GetAsync(requestUri);

            var results = await m.Content.ReadAsStringAsync();

            return StoreGenresInCache(results);

        }

        private bool StoreGenresInCache(string results)
        {
            var movies = JsonConvert.DeserializeObject<Genres>(results);

            //Here, all genres are chached, the cache memory will store them for 60 days after that they have to be stored again.. 
            Barrel.Current.Add("Genres.Cached", movies, TimeSpan.FromDays(60));

            return true;
        }

        private bool StoreInCache(string results)
        {

            var movies = JsonConvert.DeserializeObject<Movie>(results);

            //Here, all movies are chached, the cache memory will store them for 24hrs.. after that they have to be stored again.. 
            Barrel.Current.Add("Movies.Cached", movies, TimeSpan.FromDays(1));
                
            return true;
        }
    }
}
