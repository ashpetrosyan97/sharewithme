using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Core.SharedFiles
{
    public interface ISharedFileManager
    {
        Task<SharedFileEntity> CreateAsync(SharedFileEntity input);
        Task UpdateAsync(SharedFileEntity input);
        Task DeleteAsync(SharedFileEntity input);
        Task<SharedFileEntity> GetAsync(long id);
        Task<List<SharedFileEntity>> GetAll(long userId);
        Task<List<SharedFileEntity>> GetDeletedFiles(long userId);

    }
}
