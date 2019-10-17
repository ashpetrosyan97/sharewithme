using SWM.Core;
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
using SWM.Core.Repositories;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SWM.EFCore.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly SWMDbContext _context;

        public Repository(SWMDbContext context)
        {
            _context = context;
        }

        protected virtual DbSet<TEntity> Entities => _context.Set<TEntity>();

        public async virtual Task<EntityEntry<TEntity>> InsertAsync(TEntity item)
        {
            return await Entities.AddAsync(item);
        }

        public virtual void Update(TEntity item)
        {
            _context.Update(item);
        }

        public virtual void Delete(TEntity item)
        {
            Entities.Remove(item);
        }
        public virtual IQueryable<TEntity> GetAllEntities()
        {
            return Entities;
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null, bool disableTrack = false)
        {
            IQueryable<TEntity> query = GetAllEntities();
            query = query.Where(predicate);

            if (includes != null)
                query = includes(query);

            if (disableTrack)
                return await query.AsNoTracking().FirstOrDefaultAsync();
            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> Count(Expression<Func<TEntity, bool>> predicate = null)
        {
            return await GetAllEntities().CountAsync(predicate);
        }
    }
}
