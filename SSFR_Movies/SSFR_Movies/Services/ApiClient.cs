﻿using CommonServiceLocator;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using SSFR_Movies.Models;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using MonkeyCache;
using MonkeyCache.FileStore;
using SSFR_Movies.Helpers;
using Plugin.Connectivity;
using Xamarin.Forms;

namespace SSFR_Movies.Services
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

                var m = await App.httpClient.GetAsync(requestUri);

                var results = await m.Content.ReadAsStringAsync();

                return StoreInCache(results);
            }
            catch (Exception e)
            {
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

                var m = await App.httpClient.GetAsync(requestUri);

                var results = await m.Content.ReadAsStringAsync();

                var movie = JsonConvert.DeserializeObject<Movie>(results);

                return movie;
            }
            catch (Exception e)
            {

            }

            return new Movie();
        }

        //public async Task<MovieTrailer> GetMovieTrailer()
        //{

        //}
        
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

                var m = await App.httpClient.GetAsync(requestUri);

                var results = await m.Content.ReadAsStringAsync();

                return StoreMovieByGenresInCache(results);

            }
            catch (Exception e)
            {
                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection or maybe that movie doesn't exists!");

                    return false;
                });

                return false;
            }

        }

        private bool StoreMovieByGenresInCache(string results)
        {
            var movies = JsonConvert.DeserializeObject<Movie>(results);

            //Here, all genres are chached, the cache memory will store them for 60 days after that they have to be stored again.. 
            Barrel.Current.Add("MoviesByXGenre.Cached", movies, TimeSpan.FromMinutes(5));

            return true;
        }

        public async Task<bool> GetAndStoreMovieGenresAsync()
        {
            
            await Task.Yield();

            var requestUri = $"/3/genre/movie/list?api_key={API_KEY}&language={LANG}";

            var m = await App.httpClient.GetAsync(requestUri);

            var results = await m.Content.ReadAsStringAsync();

            return StoreGenresInCache(results);

        }

        public async Task<MovieVideo> GetMovieVideosAsync(int id)
        {

            await Task.Yield();

            var requestUri = $"/3/movie/{id}/videos?api_key={API_KEY}&language={LANG}";

            var m = await App.httpClient.GetAsync(requestUri);

            var results = await m.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<MovieVideo>(results);

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
            try
            {
                var movies = JsonConvert.DeserializeObject<Movie>(results);

                //Here, all movies are chached, the cache memory will store them for 24hrs.. after that they have to be stored again.. 
                Barrel.Current.Add("Movies.Cached", movies, TimeSpan.FromDays(1));

                return true;
            }
            catch (Exception e)
            {
                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection or maybe that movie doesn't exists!");

                    return false;
                });

                return false;
            }

        }
    }
}
