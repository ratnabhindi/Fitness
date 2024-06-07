using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Application.Common
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> order = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
                bool disableTracking = true);

        T GetById(object id);
        T GetByIdAsync(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> order = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
                bool disableTracking = true);

        Task Add(T entity);
        Task<T> AddAsync(T entity);

        Task Uodate(T entity);
        Task<T> UpdateAsyn(T entity);

        Task Delete(T entity);
        Task<T> DeleteAsync(T entity);

        void DeleteRange(List<T> entities);

    }
}
