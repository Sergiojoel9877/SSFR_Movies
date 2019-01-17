using MonkeyCache.FileStore;
using Newtonsoft.Json;
using Plugin.Connectivity;
using SSFR_Movies.Helpers;
using SSFR_Movies.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace SSFR_Movies.Services
{
    /// <summary>
    /// To get all movies and store them in cache. 
    /// </summary>
    [Preserve(AllMembers = true)]
    public class ApiClient
    {
        private const string API_KEY = "766bc32f686bc7f4d8e1c4694b0376a8";

        private const string LANG = "en-US";
        
        JsonSerializer serializer = new JsonSerializer();

        public async Task<bool> GetAndStoreMoviesAsync(bool include_video, CancellationTokenSource token = null, int page = 1, string sortby = "popularity.desc", bool include_adult = false, int genres = 12)
        {
            await Task.Yield();
            //App.httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            try
            {
                //Verify if internet connection is available
                if (!CrossConnectivity.Current.IsConnected)
                {
                    Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                    {
                        DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");
                        return false;
                    });

                    return false;
                }

                Settings.NextPage = page;

                var valid = genres <= 12 ? true : false;

                string _genres = valid == true ? Convert.ToString(genres) : null;

                var requestUri = $"/3/discover/movie?api_key={API_KEY}&language={LANG}&sort_by={sortby}&include_adult={include_adult.ToString().ToLower()}&include_video={include_video.ToString().ToLower()}&page={page}&with_genres={_genres}";

                var m = await App.httpClient.Value.GetAsync(requestUri);
               
                m.EnsureSuccessStatusCode();

                using (var stream = await m.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                using (var json = new JsonTextReader(reader))
                {
                    return StoreInCache(json);
                }
                              
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.InnerException);
                return false;
            }
            
        }

        public async Task<Movie> SearchMovieByName(string name, bool include_adult = false)
        {

            await Task.Yield();
            //App.httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            try
            {
                //Verify if internet connection is available
                if (!CrossConnectivity.Current.IsConnected)
                {
                    Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                    {
                        DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");
                        return false;
                    });
                    return null;
                }

                var requestUri = $"/3/search/movie?api_key={API_KEY}&language={LANG}&query={name}&include_adult={include_adult.ToString().ToLower()}";

                var m = await App.httpClient.Value.GetAsync(requestUri);
                m.EnsureSuccessStatusCode();

                using (var stream = await m.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                using (var json = new JsonTextReader(reader))
                {
                    return serializer.Deserialize<Movie>(json);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.InnerException);
            }
            return new Movie();
        }
        
        //CREATE GETMOVIESBYGENRE
        public async Task<bool> GetAndStoreMoviesByGenreAsync(int genre, bool include_video, string sortby = "popularity.desc", bool include_adult = false, int page = 1)
        {

            await Task.Yield();
           //App.httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            //Verify if internet connection is available
            if (!CrossConnectivity.Current.IsConnected)
            {
                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");
                    return false;
                });
                return false;
            }
            try
            {

                var valid = genre <= 12 ? true : false;

                string _genres = valid == true ? Convert.ToString(genre) : null;

                var requestUri = $"/3/discover/movie?api_key={API_KEY}&language={LANG}&sort_by={sortby}&include_adult={include_adult.ToString().ToLower()}&include_video={include_video.ToString().ToLower()}&page={page}&with_genres={genre}";
                
                var m = await App.httpClient.Value.GetAsync(requestUri);
                m.EnsureSuccessStatusCode();

                using (var stream = await m.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                using (var json = new JsonTextReader(reader))
                {
                    return StoreMovieByGenresInCache(json);
                }
            }
            catch (Exception e)
            {
                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("An error has ocurred!");
                    Debug.WriteLine("Error: " + e.InnerException);
                    return false;
                });

                return false;
            }

        }

        private bool StoreMovieByGenresInCache(JsonTextReader results)
        {
            var movies = serializer.Deserialize<Movie>(results);

            //var PosterPath = "https://image.tmdb.org/t/p/w370_and_h556_bestv2";

            //var Backdroppath = "https://image.tmdb.org/t/p/w1066_and_h600_bestv2";

            //movies.Results.ForEach(r =>
            //{
            //    r.PosterPath = PosterPath + r.PosterPath;
            //});

            //movies.Results.ForEach(e =>
            //{
            //    e.BackdropPath = Backdroppath + e.BackdropPath;
            //});

            //Here, all genres are chached, the cache memory will store them for 5 minutes after that they have to be stored again.. 
            Barrel.Current.Add("MoviesByXGenre.Cached", movies, TimeSpan.FromMinutes(5));

            return true;
        }

        public async Task<bool> GetAndStoreMovieGenresAsync()
        {
            
            await Task.Yield();
            //App.httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            var requestUri = $"/3/genre/movie/list?api_key={API_KEY}&language={LANG}";

            var m = await App.httpClient.Value.GetAsync(requestUri);
            m.EnsureSuccessStatusCode();

            using (var stream = await m.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            using (var json = new JsonTextReader(reader))
            {
                return StoreGenresInCache(json);
            }
        }

        public async Task<MovieVideo> GetMovieVideosAsync(int id)
        {

            await Task.Yield();
           
            var requestUri = $"/3/movie/{id}/videos?api_key={API_KEY}&language={LANG}";
            
            var m = await App.httpClient.Value.GetAsync(requestUri);
            m.EnsureSuccessStatusCode();

            using (var stream = await m.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            using (var json = new JsonTextReader(reader))
            {
                return serializer.Deserialize<MovieVideo>(json);
            }
        }

        private bool StoreGenresInCache(JsonTextReader results)
        {
            var movies = serializer.Deserialize<Genres>(results);

            //Here, all genres are chached, the cache memory will store them for 60 days after that they have to be stored again.. 
            try
            {
                Barrel.Current.Add("Genres.Cached", movies, TimeSpan.FromDays(60));
            }
            catch (DirectoryNotFoundException)
            {
                Debug.WriteLine("No storage left");
                return false;
            }

            return true;
        }

        private bool StoreInCache(JsonTextReader results)
        {
            try
            {
                var movies = serializer.Deserialize<Movie>(results);

                //var PosterPath = "https://image.tmdb.org/t/p/w370_and_h556_bestv2";

                //var Backdroppath = "https://image.tmdb.org/t/p/w1066_and_h600_bestv2";

                //movies.Results.ForEach(r =>
                //{
                //    r.PosterPath = PosterPath + r.PosterPath;
                //});

                //movies.Results.ForEach(e =>
                //{
                //     e.BackdropPath = Backdroppath + e.BackdropPath;
                //});

                try
                {
                    //Here, all movies are chached, the cache memory will store them for 24hrs.. after that they have to be stored again.. 
                    Barrel.Current.Add("Movies.Cached", movies, TimeSpan.FromDays(1));
                }
                catch (DirectoryNotFoundException e)
                {
                    Debug.WriteLine("No storage left" + e.InnerException);
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("An error has ocurred!");
                    Debug.WriteLine("Error: " + e.InnerException);
                    return false;
                });

                return false;
            }
        }
    }
}
