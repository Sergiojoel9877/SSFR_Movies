using Newtonsoft.Json;
using Realms;
using SSFR_Movies.Helpers;
using SSFR_Movies.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSFR_Movies.Services
{
    /// <summary>
    /// To get all movies and store them in cache. 
    /// </summary>
    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    public class ApiClient
    {
        private const string API_KEY = "766bc32f686bc7f4d8e1c4694b0376a8";

        private const string API_KEY_Streaming = "jw9HuWd8ChouvpUk";

        private const string LANG = "en-US";

        readonly Lazy<JsonSerializer> serializer = new Lazy<JsonSerializer>(() => new JsonSerializer());

        private readonly Lazy<Realm> realm = new Lazy<Realm>(()=> Realm.GetInstance());

        #region MoviesCacheFunctionsEtcRegion

        public async Task<bool> GetAndStoreMoviesAsync(bool include_video, int page = 1, string sortby = "popularity.desc", bool include_adult = false, int genres = 12)
        {
            new SynchronizationContextRemover();

            try
            {
                //Verify if internet connection is available
                if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
                {
                    return false;
                }

                Settings.NextPage = page;

                var valid = genres <= 12 ? true : false;

                string _genres = valid == true ? Convert.ToString(genres) : null;

                var requestUri = $"/3/discover/movie?api_key={API_KEY}&language={LANG}&sort_by={sortby}&include_adult={include_adult.ToString().ToLower()}&include_video={include_video.ToString().ToLower()}&page={page}&with_genres={_genres}";

                using (var m = await App.HttpClient.GetAsync(requestUri))
                {
                    m.EnsureSuccessStatusCode();
                    using (var stream = await m.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(stream))
                    using (var json = new JsonTextReader(reader))
                    {
                        return await StoreInCache(json);
                    }
                };
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.InnerException);
                return false;
            }

        }

        public async Task<Movie> SearchMovieByName(string name, bool include_adult = false)
        {
            new SynchronizationContextRemover();

            try
            {
                //Verify if internet connection is available
                if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
                {
                    return null;
                }

                var requestUri = $"/3/search/movie?api_key={API_KEY}&language={LANG}&query={name}&include_adult={include_adult.ToString().ToLower()}";

                using (var m = await App.HttpClient.GetAsync(requestUri))
                {
                    m.EnsureSuccessStatusCode();
                    using (var stream = await m.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(stream))
                    using (var json = new JsonTextReader(reader))
                    {
                        return await DeserializeMovieAsync(json);
                    }
                };
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.InnerException);
            }
            return new Movie();
        }

        private Task<Movie> DeserializeMovieAsync(JsonTextReader json)
        {
            var tcs = new TaskCompletionSource<Movie>();
            tcs.SetResult(serializer.Value.Deserialize<Movie>(json));
            return tcs.Task;
        }

        //CREATE GETMOVIESBYGENRE
        public async Task<bool> GetAndStoreMoviesByGenreAsync(int genre, bool include_video, string sortby = "popularity.desc", bool include_adult = false, int page = 1)
        {
            new SynchronizationContextRemover();

            //Verify if internet connection is available
            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                return false;
            }
            try
            {
                var valid = genre <= 12 ? true : false;

                string _genres = valid == true ? Convert.ToString(genre) : null;

                var requestUri = $"/3/discover/movie?api_key={API_KEY}&language={LANG}&sort_by={sortby}&include_adult={include_adult.ToString().ToLower()}&include_video={include_video.ToString().ToLower()}&page={page}&with_genres={genre}";

                using (var m = await App.HttpClient.GetAsync(requestUri))
                {
                    m.EnsureSuccessStatusCode();
                    using (var stream = await m.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(stream))
                    using (var json = new JsonTextReader(reader))
                    {
                        return await StoreMovieByGenresInCache(json);
                    }
                };
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

        private Task<bool> StoreMovieByGenresInCache(JsonTextReader results)
        {
            var tcs = new TaskCompletionSource<bool>();
            var movies = serializer.Value.Deserialize<Movie>(results);

            realm.Value.Write(() => realm.Value.Add(movies, true));
            tcs.SetResult(true);

            return tcs.Task;
        }

        public async Task<bool> GetAndStoreMovieGenresAsync()
        {
            new SynchronizationContextRemover();
            
            var requestUri = $"/3/genre/movie/list?api_key={API_KEY}&language={LANG}";

            HttpResponseMessage m;
            using (m = await App.HttpClient.GetAsync(requestUri))
            {
                m.EnsureSuccessStatusCode();
                using (var stream = await m.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                using (var json = new JsonTextReader(reader))
                {
                    return await StoreGenresInCache(json);
                }
            };
        }

        public async Task<MovieVideo> GetMovieVideosAsync(int id)
        {
            new SynchronizationContextRemover();

            var requestUri = $"/3/movie/{id}/videos?api_key={API_KEY}&language={LANG}";

            HttpResponseMessage m;
            using (m = await App.HttpClient.GetAsync(requestUri))
            {
                m.EnsureSuccessStatusCode();
                using (var stream = await m.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                using (var json = new JsonTextReader(reader))
                {
                    return await DeserializeMovieVideoAsync(json);
                }
            }
        }

        private Task<MovieVideo> DeserializeMovieVideoAsync(JsonTextReader json)
        {
            var tcs = new TaskCompletionSource<MovieVideo>();
            tcs.SetResult(serializer.Value.Deserialize<MovieVideo>(json));
            return tcs.Task;
        }

        private Task<bool> StoreGenresInCache(JsonTextReader results)
        {
            var tcs = new TaskCompletionSource<bool>();
            var movies = serializer.Value.Deserialize<Genres>(results);

            //Here, all genres are chached, the cache memory will store them for 60 days after that they have to be stored again.. 
            try
            {
                realm.Value.Write(() => realm.Value.Add(movies, true));
                tcs.SetResult(true);
            }
            catch (DirectoryNotFoundException)
            {
                Debug.WriteLine("No storage left");
                tcs.SetResult(false);
                return tcs.Task;
            }

            return tcs.Task;
        }

        private Task<bool> StoreInCache(JsonTextReader results)
        {
            var tcs = new TaskCompletionSource<bool>();

            try
            {
                var movies = serializer.Value.Deserialize<Movie>(results);
         
                try
                {
                    realm.Value.Write(() => realm.Value.Add(movies, true));
                    tcs.SetResult(true);
                }
                catch (DirectoryNotFoundException e)
                {
                    Debug.WriteLine("No storage left" + e.InnerException);
                    tcs.SetResult(false);
                    return tcs.Task;
                }
                return tcs.Task;
            }
            catch (Exception e)
            {
                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("An error has ocurred!");
                    Debug.WriteLine("Error: " + e.InnerException);
                    return false;
                });
                tcs.SetResult(false);
                return tcs.Task;
            }
        }
        #endregion


        #region MovieStreammingFunctionsRegion

        public string PlayMovieByNameAndYear(string Title, string Year)
        {
            string url = $"https://videospider.in/getvideo?key={API_KEY_Streaming}&title=+{Title.ToLower()}&year={Year}";
            return url;
        }

        #endregion
    }
}
