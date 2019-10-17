using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Core.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Get all entities from database
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> GetAllEntities();

        /// <summary>
        /// Insert entity to database
        /// </summary>
        /// <param name="entity"></param>
        Task<EntityEntry<TEntity>> InsertAsync(TEntity entity);

        /// <summary>
        /// Update entity in database
        /// </summary>
        /// <param name="entity"></param>
        void Update(TEntity entity);

        /// <summary>
        /// Get query for entity
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <param name="disableTrack"></param>
        /// <returns></returns>
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null, bool disableTrack = false);


        /// <summary>
        /// Delete entity from database
        /// </summary>
        /// <param name="entity"></param>
        void Delete(TEntity entity);
        /// <summary>
        /// Asynchronously gets count of entities by given predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<int> Count(Expression<Func<TEntity, bool>> predicate = null);
    }

}
