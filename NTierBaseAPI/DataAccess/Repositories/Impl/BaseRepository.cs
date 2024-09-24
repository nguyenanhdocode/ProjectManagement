using DataAccess.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
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

        public void DeleteRange(IList<T> entities)
        {
            DbSet.RemoveRange(entities);
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
            var param = Expression.Parameter(typeof(T));
            var left = Expression.Property(param, "Id");
            var right = Expression.Constant(id);
            var equal = Expression.Equal(left, right);
            var expression = Expression.Lambda<Func<T, bool>>(equal, param);

            return await DbSet.SingleOrDefaultAsync(expression);
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            var param = Expression.Parameter(typeof(T));
            var left = Expression.Property(param, "Id");
            var right = Expression.Constant(id);
            var equal = Expression.Equal(left, right);
            var expression = Expression.Lambda<Func<T, bool>>(equal, param);

            return await DbSet.SingleOrDefaultAsync(expression);
        }

        public async Task<List<T>> GetManyAsync(Expression<Func<T, bool>> expression)
        {
            return await DbSet.Where(expression).ToListAsync();
        }

        public void Update(T entity)
        {
            DbSet.Update(entity);
        }
    }
}
