using Microsoft.EntityFrameworkCore;
using SWM.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Core.SharedFiles
{
    public class SharedFileManager : ISharedFileManager
    {
        private readonly IUnitOfWork uow;
        public SharedFileManager(IUnitOfWork uow)
        {
            this.uow = uow;
        }
        public async Task<SharedFileEntity> CreateAsync(SharedFileEntity input)
        {
            return await uow.Repository<SharedFileEntity>().InsertAsync(input);
        }

        public async Task DeleteAsync(SharedFileEntity input)
        {
            await uow.Repository<SharedFileEntity>().DeleteAsync(input);
        }

        public async Task<List<SharedFileEntity>> GetAll(long userId)
        {
            return await uow.Repository<SharedFileEntity>().GetAllIncluding(f => f.File, f => f.User)
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public Task<SharedFileEntity> GetAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<List<SharedFileEntity>> GetDeletedFiles(long userId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(SharedFileEntity input)
        {
            throw new NotImplementedException();
        }
    }
}
