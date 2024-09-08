using DataAccess.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Impl
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        public DbSet<T> DbSet
        {
            get
            {
                return this.Dbcontext.Set<T>();
            }
        }

        public AppDbContext Dbcontext { get; set; }

        public async Task Add(T entity)
        {
            await DbSet.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }

        public async Task<T?> Get(Expression<Func<T, bool>> expression)
        {
            return await DbSet.SingleOrDefaultAsync(expression);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await DbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            return await DbSet.FirstOrDefaultAsync(p => ((string)p.GetType().GetProperty("Id").GetValue(p) == id));
        }

        public async Task<List<T>> GetManyAsync(Expression<Func<T, bool>> expression)
        {
            return await DbSet.Where(expression).ToListAsync();
        }

        public Task Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
