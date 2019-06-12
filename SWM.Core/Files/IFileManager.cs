using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Core.Files
{
    public interface IFileManager
    {
        Task<FileEntity> CreateAsync(FileEntity input);
        Task UpdateAsync(FileEntity input);
        Task RestoreAsync(FileEntity input);
        Task SoftDeleteAsync(FileEntity input);
        Task DeleteAsync(FileEntity input);
        Task<FileEntity> GetAsync(long id);
        Task<bool> IsExist(long parentId, long userId, string name);
        Task<List<FileEntity>> GetAll(long userId);
        Task<List<FileEntity>> GetDeletedFiles(long userId);
    }
}
