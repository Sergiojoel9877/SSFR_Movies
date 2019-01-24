using MonkeyCache.FileStore;
using Newtonsoft.Json;
using Plugin.Connectivity;
using SSFR_Movies.Helpers;
using SSFR_Movies.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
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

        private const string API_KEY_Streaming = "jw9HuWd8ChouvpUk";

        private const string LANG = "en-US";
               
        Lazy<JsonSerializer> serializer = new Lazy<JsonSerializer>(() => new JsonSerializer());

        #region MoviesCacheFunctionsEtcRegion

        public async Task<bool> GetAndStoreMoviesAsync(bool include_video, CancellationTokenSource token = null, int page = 1, string sortby = "popularity.desc", bool include_adult = false, int genres = 12)
        {
            await Task.Yield();
  
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

            await Task.Yield();

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
            var movies = serializer.Value.Deserialize<Movie>(results);
            
            //Here, all genres are chached, the cache memory will store them for 5 minutes after that they have to be stored again.. 
            Barrel.Current.Add("MoviesByXGenre.Cached", movies, TimeSpan.FromMinutes(5));

            return true;
        }

        public async Task<bool> GetAndStoreMovieGenresAsync()
        {
            
            await Task.Yield();

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
                return serializer.Value.Deserialize<MovieVideo>(json);
            }
        }

        private bool StoreGenresInCache(JsonTextReader results)
        {
            var movies = serializer.Value.Deserialize<Genres>(results);

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
                var movies = serializer.Value.Deserialize<Movie>(results);

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
        #endregion


        #region MovieStreammingFunctionsRegion

        const string UserAgent = "Mozilla / 5.0(Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.1";
        const string NumbersJs = "https://openload.co/assets/js/obfuscator/n.js";

        public string PlayMovieByNameAndYear(string Title, string Year)
        {
            string url = $"https://videospider.in/getvideo?key={API_KEY_Streaming}&title=+{Title.ToLower()}&year={Year}";
            return url;
        }

        private static string GetStreamURL(string URL)
        {
            string HTML = HttpGet(URL);
            string NJs = HttpGet(NumbersJs);

            string LinkImg = Regex.Match(HTML, "src=\"data:image/png;base64,([A-Za-z0-9+/=]+?)\"").Groups[1].Value;
            string SigNums = Regex.Match(NJs, "window\\.signatureNumbers='([a-z]+?)'").Groups[1].Value;

            byte[] ImgData = Convert.FromBase64String(LinkImg);

            string ImgNums = string.Empty;

            using (MemoryStream MS = new MemoryStream(ImgData))
            {
                using (Bitmap Img = new Bitmap(MS))
                {
                    for (int Y = 0; Y < Img.Height; Y++)
                    {
                        for (int X = 0; X < Img.Width; X++)
                        {
                            Color Col = Img.GetPixel(X, Y);

                            if (Col == Color.FromRgb(0, 0, 0))
                            {
                                //Black color = end of data
                                Y = Img.Height;
                                break;
                            }

                            ImgNums += (char)Col.R;
                            ImgNums += (char)Col.G;
                            ImgNums += (char)Col.B;
                        }
                    }
                }
            }

            string[,] ImgStr = new string[10, ImgNums.Length / 200];
            string[,] SigStr = new string[10, SigNums.Length / 260];

            for (int i = 0; i < 10; i++)
            {
                //Fill Array of Image String
                for (int j = 0; j < ImgStr.GetLength(1); j++)
                {
                    ImgStr[i, j] = ImgNums.Substring(i * ImgStr.GetLength(1) * 20 + j * 20, 20);
                }

                //Fill Array of Signature Numbers
                for (int j = 0; j < SigStr.GetLength(1); j++)
                {
                    SigStr[i, j] = SigNums.Substring(i * SigStr.GetLength(1) * 26 + j * 26, 26);
                }
            }
            
            List<string> Parts = new List<string>();

            int[] Primes = { 2, 3, 5, 7 };

            foreach (int i in Primes)
            {
                string Str = string.Empty;
                float Sum = 99f; //c

                for (int j = 0; j < SigStr.GetLength(1); j++)
                {
                    for (int ChrIdx = 0; ChrIdx < ImgStr[i, j].Length; ChrIdx++)
                    {
                        if (Sum > 122f) Sum = 98f; //b

                        char Chr = (char)((int)Math.Floor(Sum));

                        if (SigStr[i, j][ChrIdx] == Chr && j >= Str.Length)
                        {
                            Str += ImgStr[i, j][ChrIdx];
                            Sum += 2.5f;
                        }
                    }
                }

                Parts.Add(Str.Replace(",", string.Empty));
            }

            string StreamURL = "https://openload.co/stream/";

            StreamURL += Parts[3] + "~";
            StreamURL += Parts[1] + "~";
            StreamURL += Parts[2] + "~";
            StreamURL += Parts[0];

            return StreamURL;
        }

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
