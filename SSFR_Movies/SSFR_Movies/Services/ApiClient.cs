using Newtonsoft.Json;
using Realms;
using SSFR_Movies.Helpers;
using SSFR_Movies.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

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
               
        Lazy<JsonSerializer> serializer = new Lazy<JsonSerializer>(() => new JsonSerializer());

        private readonly Realm realm = Realm.GetInstance();

        #region MoviesCacheFunctionsEtcRegion

        public async Task<bool> GetAndStoreMoviesAsync(bool include_video, CancellationTokenSource token = null, int page = 1, string sortby = "popularity.desc", bool include_adult = false, int genres = 12)
        {
            //await new SynchronizationContextRemover();

            await Task.Yield();

            try
            {
                //Verify if internet connection is available
                if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
                {
                    //Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                    //{
                    //    DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");
                    //    return false;
                    //});
                    await MaterialDialog.Instance.SnackbarAsync("No internet Connection", 3000);

                    return false;
                }

                Settings.NextPage = page;

                var valid = genres <= 12 ? true : false;

                string _genres = valid == true ? Convert.ToString(genres) : null;

                var requestUri = $"/3/discover/movie?api_key={API_KEY}&language={LANG}&sort_by={sortby}&include_adult={include_adult.ToString().ToLower()}&include_video={include_video.ToString().ToLower()}&page={page}&with_genres={_genres}";

                var m = await App.httpClient.GetAsync(requestUri);
               
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
            //await new SynchronizationContextRemover();

            await Task.Yield();

            try
            {
                //Verify if internet connection is available
                if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
                {
                    //Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                    //{
                    //    DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");
                    //    return false;
                    //});
                    await MaterialDialog.Instance.SnackbarAsync("No internet Connection", 3000);

                    return null;
                }

                var requestUri = $"/3/search/movie?api_key={API_KEY}&language={LANG}&query={name}&include_adult={include_adult.ToString().ToLower()}";

                var m = await App.httpClient.GetAsync(requestUri);
                m.EnsureSuccessStatusCode();

                using (var stream = await m.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                using (var json = new JsonTextReader(reader))
                {
                    return serializer.Value.Deserialize<Movie>(json);
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
            //await new SynchronizationContextRemover();

            await Task.Yield();

            //Verify if internet connection is available
            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                //Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                //{
                //    DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");
                //    return false;
                //});
                await MaterialDialog.Instance.SnackbarAsync("No internet Connection", 3000);

                return false;
            }
            try
            {

                var valid = genre <= 12 ? true : false;

                string _genres = valid == true ? Convert.ToString(genre) : null;

                var requestUri = $"/3/discover/movie?api_key={API_KEY}&language={LANG}&sort_by={sortby}&include_adult={include_adult.ToString().ToLower()}&include_video={include_video.ToString().ToLower()}&page={page}&with_genres={genre}";
                
                var m = await App.httpClient.GetAsync(requestUri);
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
            var movies = serializer.Value.Deserialize<Movie>(results);

            realm.Write(()=> realm.Add(movies, true));

            ////Here, all genres are chached, the cache memory will store them for 5 minutes after that they have to be stored again.. 
            //Barrel.Current.Add("MoviesByXGenre.Cached", movies, TimeSpan.FromMinutes(5));

            return true;
        }

        public async Task<bool> GetAndStoreMovieGenresAsync()
        {
            //await new SynchronizationContextRemover();

            await Task.Yield();

            var requestUri = $"/3/genre/movie/list?api_key={API_KEY}&language={LANG}";

            var m = await App.httpClient.GetAsync(requestUri);
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
            await new SynchronizationContextRemover();

            await Task.Yield();
           
            var requestUri = $"/3/movie/{id}/videos?api_key={API_KEY}&language={LANG}";

            var m = await App.httpClient.GetAsync(requestUri);
            m.EnsureSuccessStatusCode();

            using (var stream = await m.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            using (var json = new JsonTextReader(reader))
            {
                return serializer.Value.Deserialize<MovieVideo>(json);
            }
        }

        private bool StoreGenresInCache(JsonTextReader results)
        {
            var movies = serializer.Value.Deserialize<Genres>(results);

            //Here, all genres are chached, the cache memory will store them for 60 days after that they have to be stored again.. 
            try
            {
                realm.Write(()=> realm.Add(movies, true));
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
                var movies = serializer.Value.Deserialize<Movie>(results);
                
                try
                {
                    realm.Write(()=> realm.Add(movies, true));
                    //Here, all movies are chached, the cache memory will store them for 24hrs.. after that they have to be stored again.. 
                    //Barrel.Current.Add("Movies.Cached", movies, TimeSpan.FromDays(1));
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
        #endregion


        #region MovieStreammingFunctionsRegion

        const string UserAgent = "Mozilla/5.0 (Linux; Android 4.4; Nexus 5 Build/_BuildID_) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/30.0.0.0 Mobile Safari/537.36";
        const string NumbersJs = "https://openload.co/assets/js/obfuscator/n.js";

        public string PlayMovieByNameAndYear(string Title, string Year)
        {
            string url = $"https://videospider.in/getvideo?key={API_KEY_Streaming}&title=+{Title.ToLower()}&year={Year}";
            return url;
        }

        //public async Task<ResultDW> GetStreamURL(string URL)
        //{
        //    var obj1 = default(ResultOP);
        //    var fileID = URL.Substring(URL.Length - 11);

        //    System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient
        //    {
        //        BaseAddress = new Uri("https://api.openload.co/1")
        //    };
        //    httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);

        //    try
        //    {
        //        string request = $"/file/dlticket?file={fileID}";

        //        var m = await httpClient.GetAsync(request);

        //        var results = await m.Content.ReadAsStringAsync();

        //        obj1 = JsonConvert.DeserializeObject<ResultOP>(results);
        //    }
        //    catch (Exception E)
        //    {
        //        Debug.WriteLine($"ERROR: {E.InnerException}");
        //    }
        //    var downloadLink = await GetDownloadLink(fileID, obj1);

        //    return downloadLink;
        //}

        //public async Task<ResultDW> GetDownloadLink(string fileID, ResultOP result)
        //{
        //    HttpClient httpClient = new HttpClient();
        //    httpClient.BaseAddress = new Uri("https://api.openload.co/1");
            
        //    string request = $"/file/dl?file={fileID}&ticket={result.Ticket}&captcha_response={result.CaptchaUrl}";

        //    try
        //    {
        //        var m = await httpClient.GetAsync(request);

        //        var results = await m.Content.ReadAsStringAsync();

        //        var obj1 = JsonConvert.DeserializeObject<ResultDW>(results);
        //    }
        //    catch (Exception ER)
        //    {
        //        Debug.WriteLine($"ERROR: {ER.InnerException}");
        //    }
        //    return null;
        //}
        
        private static string HttpGet(string URL)
        {
            WebRequest Request = WebRequest.Create(URL);
            ((HttpWebRequest)Request).UserAgent = UserAgent;

            WebResponse Response = Request.GetResponse();
            StreamReader Reader = new StreamReader(Response.GetResponseStream());

            return Reader.ReadToEnd();
        }

        #endregion
    }
}
