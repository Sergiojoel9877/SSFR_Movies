using GBH_Movies_Test.Models;
using GBH_Movies_Test.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GBH_Movies_Test.Data
{
    /// <summary>
    /// Implements the CRUD methods of IDBRepository interface
    /// </summary>
    /// <typeparam name="T">Any object that inherit from BaseEntity</typeparam>
    public class DBRepository<T> : IDBRepository<T> where T : class
    {
        private readonly DatabaseContext<T> dbContext;

        public DBRepository(DatabaseContext<T> context)
        {
            dbContext = context;        
        }

        public async Task<bool> AddEntity(T obj)
        {
            await Task.Yield();

            var entity = await dbContext.Entity.AddAsync(obj);

            await dbContext.SaveChangesAsync();

            var added = obj.Id == entity.Entity.Id;

            return added;
        }

        public async Task<bool> DeleteEntity(T obj)
        {
            await Task.Yield();

            var entity = dbContext.Entity.Remove(obj);

            await dbContext.SaveChangesAsync();

            var deleted = obj.Id == entity.Entity.Id;

            return deleted;

        }

        public async Task<IEnumerable<T>> GetEntities()
        {
            await Task.Yield();

            return await dbContext.Entity.AsNoTracking().ToListAsync() ?? null;

        }

        public async Task<T> GetEntity(int id)
        {
            await Task.Yield();

            return await dbContext.Entity.FindAsync(id) ?? null;
        }

        public async Task<bool> UpdateEntity(T obj)
        {
            await Task.Yield();

            var entity = dbContext.Entity.Update(obj);

            await dbContext.SaveChangesAsync();

            bool updated = obj.Id == entity.Entity.Id;

            return updated;
        }
    }
}
