using System;
using System.Threading.Tasks;
using SSFR_Movies.Helpers;
using SSFR_Movies.Models;

namespace SSFR_Movies.Services.Abstract
{
    public interface IMovieService
    {
        event EventHandler<MovieEventArgs> OnMovieAdded;
        event EventHandler<MovieEventArgs> OnMovieRemoved;

        Task<bool> AddMovieToFavoritesList(Result result);
        Task<bool> RemoveMovieFromFavoriteList(Result result);
    }
}
