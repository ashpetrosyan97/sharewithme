using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Win32.SafeHandles;
using SWM.Core;
using SWM.Core.Repositories;
using SWM.EFCore.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SWM.EFCore.UnitOfWork
{
    public class UnitOfWork : SafeHandleZeroOrMinusOneIsInvalid, IUnitOfWork
    {
        private readonly SWMDbContext context;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        public Dictionary<Type, object> Repositories
        {
            get { return _repositories; }
            private set { Repositories = value; }
        }


        public UnitOfWork(SWMDbContext context) : base(true)
        {
            this.context = context;
        }

        protected override bool ReleaseHandle()
        {
            context?.Dispose();
            return true;
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (Repositories.Keys.Contains(typeof(TEntity)))
            {
                return Repositories[typeof(TEntity)] as IRepository<TEntity>;
            }

            IRepository<TEntity> repo = new Repository<TEntity>(context);
            Repositories.Add(typeof(TEntity), repo);
            return repo;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel)
        {
            return await context.Database.BeginTransactionAsync(isolationLevel);
        }
        public async Task<int> CommitAsync()
        {
            return await context.SaveChangesAsync();
        }
        public int Commit()
        {
            return context.SaveChanges();
        }

        public void Rollback()
        {
            context.ChangeTracker.Entries()
                .ToList()
                .ForEach(x => x.Reload());
        }
    }
}
