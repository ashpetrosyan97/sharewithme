using Microsoft.EntityFrameworkCore;
using SWM.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Core.Files
{
    public class FileManager : IFileManager
    {
        private readonly IUnitOfWork uow;
        public FileManager(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public async Task<FileEntity> CreateAsync(FileEntity input)
        {
            return await uow.Repository<FileEntity>().InsertAsync(input);

            /* var sharedParent = await uow.Repository<SharedFileEntity>().GetAsync(x => x.FileId == input.ParentId, x => x.User, x => x.File);

             if (sharedParent != null)
             {
                 foreach (var item in sharedParent.File.UsersSharedFiles)
                 {
                     await uow.Repository<SharedFileEntity>().InsertAsync(new SharedFileEntity { FileId = input.Id, UserId = item.UserId });
                 }
             }*/
        }

        public async Task SoftDeleteAsync(FileEntity input)
        {
            input.IsDeleted = true;
            input.DeletionTime = DateTime.Now;
            await uow.Repository<FileEntity>().UpdateAsync(input);
            var file = await uow.Repository<SharedFileEntity>().GetAsync(x => x.FileId == input.Id, x => x.User, x => x.File);
            if (file != null)
            {
                await uow.Repository<SharedFileEntity>().DeleteAsync(file);
            }
        }

        public async Task DeleteAsync(FileEntity input)
        {
            await uow.Repository<FileEntity>().DeleteAsync(input);
        }

        public async Task<List<FileEntity>> GetAll(long userId)
        {
            return await uow.Repository<FileEntity>().GetAll()
                .Where(x => x.OwnerId == userId)
                .Where(x => x.Path != null)
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Type)
                .ThenBy(x => x.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<FileEntity> GetAsync(long id)
        {
            return await uow.Repository<FileEntity>().GetAsyncIncluding(x => x.Id == id, q => q.Include(f => f.UsersSharedFiles).ThenInclude(u => u.User));
        }

        public async Task<bool> IsExist(long parentId, long userId, string name)
        {
            bool exists = false;
            var entities = await GetAll(userId);
            entities.ForEach(x =>
            {
                if (x.Name == name && x.ParentId == parentId && !x.IsDeleted)
                    exists = true;
            });
            return exists;
        }
        public async Task UpdateAsync(FileEntity input)
        {
            input.LastModificationTime = DateTime.Now;
            await uow.Repository<FileEntity>().UpdateAsync(input);
        }

        public async Task RestoreAsync(FileEntity input)
        {
            input.IsDeleted = false;
            await uow.Repository<FileEntity>().UpdateAsync(input);
        }

        public async Task<List<FileEntity>> GetDeletedFiles(long userId)
        {
            return await uow.Repository<FileEntity>().GetAll()
                .Where(x => x.OwnerId == userId)
                .Where(x => x.Type == FileEntityType.File)
                .Where(x => x.IsDeleted)
                .OrderBy(x => x.DeletionTime)
                .ThenBy(x => x.Name)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
