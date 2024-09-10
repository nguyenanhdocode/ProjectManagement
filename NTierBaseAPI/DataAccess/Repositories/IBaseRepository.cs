using DataAccess.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        DbSet<T> DbSet { get; }
        AppDbContext Dbcontext { get; set; }

        Task<List<T>> GetAllAsync();

        Task<List<T>> GetManyAsync(Expression<Func<T, bool>> expression);

        Task<T?> GetByIdAsync(string id);

        Task<T?> GetByIdAsync(Guid id);

        Task<T?> Get(Expression<Func<T, bool>> expression);

        Task Add(T entity);

        void Delete(T entity);

        void Update(T entity);
    }
}
