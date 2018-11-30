using SSFR_Movies.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace SSFR_Movies.Services
{
    /// <summary>
    /// DBRepository interface, it contains the CRUD Methods for the database. 
    /// </summary>
    /// <typeparam name="T">Any object that inherit from BaseEntity</typeparam>
  
    public interface IDBRepository<T> : IDisposable where T : class
    {
        Task<T> GetEntity(int id);
        Task<IEnumerable<T>> GetEntities();
        Task<bool> AddEntity(T obj);
        Task<bool> UpdateEntity(T obj);
        Task<bool> DeleteEntity(T obj);
    }
}
