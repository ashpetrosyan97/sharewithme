using SWM.Core;
using SWM.Core.Repositories;
using SWM.Core.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using SWM.EFCore;

namespace SWM.EFCore
{
    public class Repository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class
    {
        private readonly SWMDbContext _context;

        public Repository(SWMDbContext context)
        {
            _context = context;
        }

        protected virtual DbSet<TEntity> Entity => _context.Set<TEntity>();

        public async virtual Task<TEntity> InsertAsync(TEntity item)
        {
            await Entity.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async virtual Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            query = query.Where(predicate);
            foreach (var prop in includes)
            {
                query = query.Include(prop);
            }
            return await query.AsNoTracking().FirstOrDefaultAsync();
        }




        public async virtual Task UpdateAsync(TEntity item)
        {
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async virtual Task DeleteAsync(TEntity item)
        {
            Entity.Remove(item);
            await _context.SaveChangesAsync();
        }
        public virtual IQueryable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().AsQueryable();
        }

        public IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            if (propertySelectors == null)
            {
                return GetAll();
            }

            var query = GetAll();
            foreach (var propertySelector in propertySelectors)
            {
                query = query.Include(propertySelector);
            }
            return query;
        }

        public Task<TEntity> GetAsyncIncluding(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool disableTracking = true)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return query.FirstOrDefaultAsync();
        }
    }
}
