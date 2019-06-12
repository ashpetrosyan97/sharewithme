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
        private readonly IRepository<SharedFileEntity, long> _sharedFileRepository;
        public SharedFileManager(IRepository<SharedFileEntity, long> sharedFileRepository)
        {
            _sharedFileRepository = sharedFileRepository;
        }
        public async Task<SharedFileEntity> CreateAsync(SharedFileEntity input)
        {
            return await _sharedFileRepository.InsertAsync(input);
        }

        public async Task DeleteAsync(SharedFileEntity input)
        {
            await _sharedFileRepository.DeleteAsync(input);
        }

        public async Task<List<SharedFileEntity>> GetAll(long userId)
        {
            return await _sharedFileRepository.GetAllIncluding(f => f.File, f => f.User)
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
