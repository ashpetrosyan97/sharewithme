using Microsoft.EntityFrameworkCore.Storage;
using SWM.Core.Repositories;
using System.Data;
using System.Threading.Tasks;

namespace SWM.Core
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync();
        int Commit();
        void Rollback();
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;
        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel);
    }
}
