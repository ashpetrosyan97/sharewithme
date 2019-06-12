using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NReco.VideoInfo;
using SWM.Application.Files;
using SWM.Application.Users;
using SWM.Core;
using SWM.Core.Accounts;
using SWM.Core.Files;
using SWM.Core.SharedFiles;
using SWM.Core.Users;
using ShareWithMe.Common;
using ShareWithMe.Hubs;
using ShareWithMe.Models;
using Microsoft.Extensions.Logging;

namespace ShareWithMe.Controllers
{
    [Route("api/[controller]/[action]")]
    public class FilesController : Controller
    {
        private readonly IFileManager _fileManager;
        private readonly ISharedFileManager _sharedFileManager;
        private readonly IUserManager _userManager;
        private readonly IHostingEnvironment _env;
        private readonly IHubContext<ProgressHub, IProgressHub> _hub;
        ILogger<FilesController> _log;
        private readonly string StorageDirectory;
        public FilesController(IFileManager fileManager, IUserManager userManager, IHostingEnvironment env, ISharedFileManager sharedFileManager, IHubContext<ProgressHub, IProgressHub> hub, ILogger<FilesController> log)
        {
            _log = log;
            _hub = hub;
            _sharedFileManager = sharedFileManager;
            _env = env;
            _fileManager = fileManager;
            _userManager = userManager;
            StorageDirectory = Path.Combine(env.WebRootPath, AppConsts.StorageDirectory);
        }

        [HttpPost]
        [Authorize]
        public async Task<JsonResult> CreateDirectory([FromForm]CreateDirectoryDto input)
        {
            var fileEntity = CustomMapper<CreateDirectoryDto, FileEntity>.Map(input);
            long userId = long.Parse(User.Identity.Name);
            fileEntity.OwnerId = userId;
            bool exists = await _fileManager.IsExist(fileEntity.ParentId, fileEntity.OwnerId, fileEntity.Name);

            if (exists)
                return new JsonResult(new ResponseModel(message: "Error while creating directory", code: HttpStatusCode.BadRequest, success: false, errors: new List<string> { $"Folder with name {input.Name} already exists" }));
            var created = await _fileManager.CreateAsync(fileEntity);


            string DbPath = "";
            string directoryPath = "";
            string folderName = created.Id.ToString();
            if (input.ParentId == 0)
            {
                var user = await _userManager.GetAsync(u => u.Id == userId);
                directoryPath = Path.Combine(StorageDirectory, user.Username, folderName);
                DbPath = Path.Combine(AppConsts.StorageDirectory, user.Username, folderName);
            }
            else
            {
                var parent = await _fileManager.GetAsync(input.ParentId);
                directoryPath = Path.Combine(_env.WebRootPath, parent.Path, folderName);
                DbPath = Path.Combine(parent.Path, folderName);
            }

            Directory.CreateDirectory(directoryPath);
            fileEntity.Path = DbPath;
            await _fileManager.UpdateAsync(fileEntity);
            return new JsonResult(new ResponseModel(message: "Succesfully created"));

        }

        [Authorize]
        [HttpGet]
        public async Task<JsonResult> GetAll()
        {
            long userId = long.Parse(User.Identity.Name);
            var Entities = await _fileManager.GetAll(userId);
            if (Entities == null)
            {
                return new JsonResult(new ResponseModel());
            }
            List<DirectoryDto> files = CustomMapper<FileEntity, DirectoryDto>.MapList(Entities);
            return new JsonResult(new ResponseModel(data: new { files }));
        }

        [Authorize]
        [HttpGet]
        public async Task<JsonResult> GetDeletedFiles()
        {
            long userId = long.Parse(User.Identity.Name);
            var Entities = await _fileManager.GetDeletedFiles(userId);
            List<DeletedFileDto> files = CustomMapper<FileEntity, DeletedFileDto>.MapList(Entities);
            return new JsonResult(new ResponseModel(data: new { files }));
        }

        [HttpPut]
        [Authorize]
        public async Task<JsonResult> Rename([FromForm]RenameDirectoryDto input)
        {
            var entity = await _fileManager.GetAsync(input.Id);
            bool exists = await _fileManager.IsExist(entity.ParentId, entity.OwnerId, input.Name);
            if (!exists)
            {
                entity.Name = input.Name;
                await _fileManager.UpdateAsync(entity);
                return new JsonResult(new ResponseModel(message: "Success"));
            }
            return new JsonResult(new ResponseModel(message: "Error while renaming", success: false, errors: new List<string> { $"Item with name {input.Name} already exists" }));
        }

        [HttpPatch]
        [Authorize]
        public async Task<JsonResult> Move([FromForm]MoveFileDto input)
        {
            var entity = await _fileManager.GetAsync(input.Id);
            var Owner = await _userManager.GetAsync(x => x.Id == entity.OwnerId);
            var newParent = await _fileManager.GetAsync(input.ParentId);
            if (newParent != null && newParent.Type == FileEntityType.File)
            {
                return new JsonResult(new ResponseModel(message: "Error while moving", success: false, errors: new List<string> { $" Destination must be directory" }));
            }
            string newParentPath = input.ParentId != 0 ? newParent.Path
                : Path.Combine(AppConsts.StorageDirectory, Owner.Username);
            var oldParentPath = entity.ParentId != 0 ? (await _fileManager.GetAsync(entity.ParentId)).Path
                : Path.Combine(AppConsts.StorageDirectory, Owner.Username);

            bool exists = await _fileManager.IsExist(input.ParentId, entity.OwnerId, entity.Name);
            string newDbPath = Path.Combine(newParentPath, entity.Type == FileEntityType.Folder ? entity.Id.ToString() : Path.GetFileName(entity.Path));
            if (!exists)
            {
                if (newParentPath != oldParentPath)
                {
                    try
                    {
                        Directory.Move(Path.Combine(_env.WebRootPath, oldParentPath, entity.Type == FileEntityType.Folder ? entity.Id.ToString() : Path.GetFileName(entity.Path)),
                           Path.Combine(_env.WebRootPath, newDbPath));
                    }
                    catch (Exception ex)
                    {
                        return new JsonResult(new ResponseModel(message: "Error while moving", success: false, errors: new List<string> { $"Something went wrong" }));
                    }
                }
                entity.ParentId = input.ParentId;
                entity.Path = newDbPath;
                await _fileManager.UpdateAsync(entity);
                return new JsonResult(new ResponseModel(message: "Success"));
            }
            return new JsonResult(new ResponseModel(message: "Error while moving", success: false, errors: new List<string> { $" Destination source contains item with name {entity.Name}" }));
        }

        [HttpDelete]
        [Authorize]
        public async Task<JsonResult> Delete(long id)
        {
            var entity = await _fileManager.GetAsync(id);
            if (entity == null)
            {
                return new JsonResult(new ResponseModel(success: false));
            }
            await _fileManager.SoftDeleteAsync(entity);

            if (entity.Type == FileEntityType.Folder)
            {
                foreach (var item in await _fileManager.GetAll(entity.OwnerId))
                {
                    if (item.Path.StartsWith($"{entity.Path}\\"))
                    {
                        await _fileManager.SoftDeleteAsync(item);
                    }
                }
            }

            long userId = long.Parse(User.Identity.Name);
            var user = await _userManager.GetAsync(u => u.Id == userId);
            double usedSpace = await Dir.GetUsedSpace(userId, _env, _fileManager) / 1024;
            double percent = 100 * usedSpace / (user.Account.StorageSize / 1024);
            string connId = ProgressHub.Connections.Where(x => x.Key == userId).First().Value;
            await _hub.Clients.Client(connId).SendStorageInfo(usedSpace, percent);

            return new JsonResult(new ResponseModel());
        }
        [HttpGet]
        [Authorize]
        public async Task<JsonResult> Get(long id)
        {
            var entity = await _fileManager.GetAsync(id);
            if (entity == null)
            {
                return new JsonResult(new ResponseModel(success: false));
            }
            var file = CustomMapper<FileEntity, FileDto>.Map(entity);
            file.Size = new FileInfo(Path.Combine(_env.WebRootPath, file.Path)).Length / Math.Pow(1024, 2);
            //file.Duration = Dir.GetVideoDuration(Path.Combine(_env.WebRootPath, file.Path));
            return new JsonResult(new ResponseModel(data: file));
        }

        [HttpPut]
        [Authorize]
        public async Task<JsonResult> Restore([FromForm]RestoreFileDto input)
        {
            var entity = await _fileManager.GetAsync(input.Id);
            var Owner = await _userManager.GetAsync(u => u.Id == entity.OwnerId);
            if (entity == null)
            {
                return new JsonResult(new ResponseModel(message: "Error while restoring file", code: HttpStatusCode.BadRequest, success: false, errors: new List<string> { "Something went wrong" }));
            }

            double freeSpasce = await Dir.GetFreeSpase(Owner, _env, _fileManager);
            double fileSize = new FileInfo(Path.Combine(_env.WebRootPath, entity.Path)).Length / Math.Pow(1024, 2);
            if (freeSpasce < fileSize)
            {
                return new JsonResult(new ResponseModel(message: "Error while restoring file", code: HttpStatusCode.BadRequest, success: false, errors: new List<string> { "File is too big.You haven't enough free space" }));
            }

            try
            {
                var newParent = await _fileManager.GetAsync(input.ParentId);
                if (newParent != null && newParent.Type == FileEntityType.File)
                {
                    return new JsonResult(new ResponseModel(message: "Error while restoring", success: false, errors: new List<string> { $" Destination must be directory" }));
                }
                string newParentPath = input.ParentId != 0 ? newParent.Path
                    : Path.Combine(AppConsts.StorageDirectory, Owner.Username);
                var oldParentPath = entity.ParentId != 0 ? (await _fileManager.GetAsync(entity.ParentId)).Path
                    : Path.Combine(AppConsts.StorageDirectory, Owner.Username);

                bool exists = await _fileManager.IsExist(input.ParentId, entity.OwnerId, entity.Name);
                string newDbPath = Path.Combine(newParentPath, entity.Type == FileEntityType.Folder ? entity.Id.ToString() : Path.GetFileName(entity.Path));

                if (!exists)
                {
                    if (newParentPath != oldParentPath)
                    {
                        try
                        {
                            Directory.Move(Path.Combine(_env.WebRootPath, oldParentPath, entity.Type == FileEntityType.Folder ? entity.Id.ToString() : Path.GetFileName(entity.Path)),
                               Path.Combine(_env.WebRootPath, newDbPath));
                        }
                        catch (Exception ex)
                        {
                            return new JsonResult(new ResponseModel(message: "Error while restoring", success: false, errors: new List<string> { $"Something went wrong" }));
                        }
                    }
                    entity.ParentId = input.ParentId;
                    entity.Path = newDbPath;

                    await _fileManager.RestoreAsync(entity);
                    return new JsonResult(new ResponseModel(message: "Succesfully restored"));

                }
                return new JsonResult(new ResponseModel(message: "Error while restoring", success: false, errors: new List<string> { $"Item with name {entity.Name} already exists" }, code: HttpStatusCode.Conflict));

            }
            catch (Exception e)
            {
                return new JsonResult(new ResponseModel(message: "Error while restoring", success: false, errors: new List<string> { $"Something went wrong" }));
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<JsonResult> GetSharedFiles()
        {
            long userId = long.Parse(User.Identity.Name);
            var sharedFiles = (await _sharedFileManager.GetAll(userId)).ToList();
            var user = await _userManager.GetAsync(u => u.Id == userId);
            var files = new List<FileEntity>();

            foreach (var entity in sharedFiles)
            {
                files.Add(entity.File);
            }
            List<SharedFileDto> entities = CustomMapper<FileEntity, SharedFileDto>.MapList(files);
            entities.ForEach(x =>
            {
                x.Duration = Dir.GetVideoDuration(Path.Combine(_env.WebRootPath, x.Path));
                x.Owner = CustomMapper<UserEntity, UserDto>.Map(_userManager.GetAsync(u => u.Id == x.UserId).Result);
                x.Size = new FileInfo(Path.Combine(_env.WebRootPath, x.Path)).Length / Math.Pow(1024, 2);
            });
            return new JsonResult(new ResponseModel(message: "Success", data: new { files = entities }));
        }

        [HttpPost]
        [Authorize]
        public async Task<JsonResult> Share(ShareFileDto input)
        {

            var file = await _fileManager.GetAsync(input.FileId);
            var allFiles = await _fileManager.GetAll(file.OwnerId);
            foreach (var userfile in file.UsersSharedFiles.ToList())
            {
                if (!input.UserId.Contains(userfile.UserId))
                {
                    await _sharedFileManager.DeleteAsync(userfile);

                    /*if (file.Type == FileEntityType.Folder)
                    {
                        foreach (var item in allFiles.ToList())
                        {
                            if (item.Path.StartsWith($"{file.Path}\\"))
                            {
                                await _sharedFileManager.DeleteAsync(new SharedFileEntity { UserId = userfile.UserId, FileId = item.Id });
                            }
                        }
                    }*/
                }
            }

            foreach (var userId in input.UserId)
            {
                if (file.UsersSharedFiles.Count == 0 || file.UsersSharedFiles.ToList().Any(u => u.UserId != userId))
                {
                    await _sharedFileManager.CreateAsync(new SharedFileEntity { FileId = input.FileId, UserId = userId });

                    /*if (file.Type == FileEntityType.Folder)
                    {
                        foreach (var item in allFiles.ToList())
                        {
                            if (item.Path.StartsWith($"{file.Path}\\"))
                            {
                                await _sharedFileManager.CreateAsync(new SharedFileEntity { FileId = item.Id, UserId = userId });
                            }
                        }
                    }*/
                }
            }
            return new JsonResult(new ResponseModel(message: "Success"));
        }


        [HttpPost]
        [DisableRequestSizeLimit]
        [Authorize]
        public async Task<JsonResult> UploadFile(UploadFileDto input)
        {
            string ext = Path.GetExtension(input.File.FileName);
            if (!AppConsts.Extentions.Contains(ext))
            {
                return new JsonResult(new ResponseModel(message: "Error while uploading file", code: HttpStatusCode.BadRequest, success: false, errors: new List<string> { "Wrong file type selected" }));
            }
            var fileEntity = CustomMapper<UploadFileDto, FileEntity>.Map(input);

            long userId = long.Parse(User.Identity.Name);
            var user = await _userManager.GetAsync(x => x.Id == userId);
            fileEntity.Type = FileEntityType.File;
            fileEntity.OwnerId = userId;
            bool exists = await _fileManager.IsExist(fileEntity.ParentId, fileEntity.OwnerId, input.Name);
            if (exists)
                return new JsonResult(new ResponseModel(message: "Error while uploading file", code: HttpStatusCode.BadRequest, success: false,
                    errors: new List<string> { $"File with name {input.Name} already exists" }));

            double freeSpasce = await Dir.GetFreeSpase(user, _env, _fileManager);
            double fileSize = input.File.Length / Math.Pow(1024, 2);
            if (freeSpasce < fileSize)
            {
                return new JsonResult(new ResponseModel(message: "Error while uploading file", code: HttpStatusCode.BadRequest, success: false, errors: new List<string> { "File is too big.You haven't enough free space" }));
            }

            string connId = ProgressHub.Connections.Where(x => x.Key == fileEntity.OwnerId).First().Value;
            try
            {
                string DbPath = "";
                string FilePath = "";
                string fileName = $"{Guid.NewGuid().ToString()}{ext}";
                if (input.ParentId == 0)
                {
                    FilePath = Path.Combine(StorageDirectory, user.Username, fileName);
                    DbPath = Path.Combine(AppConsts.StorageDirectory, user.Username, fileName);
                }
                else
                {
                    var parent = await _fileManager.GetAsync(input.ParentId);
                    FilePath = Path.Combine(_env.WebRootPath, parent.Path, fileName);
                    DbPath = Path.Combine(parent.Path, fileName);
                }

                byte[] buffer;

                if (user.Account.Type == AccountTypes.Free)
                    buffer = new byte[16 * 1024];
                else
                    buffer = new byte[10240 * 1024];


                using (FileStream output = System.IO.File.Create(FilePath))
                {
                    using (Stream sm = input.File.OpenReadStream())
                    {
                        long totalReadBytes = 0;
                        int readBytes;

                        while ((readBytes = sm.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            await output.WriteAsync(buffer, 0, readBytes);
                            totalReadBytes += readBytes;
                            double percent = Math.Round(100 * (double)totalReadBytes / input.File.Length);
                            await _hub.Clients.Client(connId).SendUploadPercent(input.Uid, percent);
                        }
                    }
                }

                fileEntity.Path = DbPath;
                await _fileManager.CreateAsync(fileEntity);
                return new JsonResult(new ResponseModel(message: "Succesfully uploaded"));
            }
            catch (Exception ex)
            {
                if (fileEntity.Id != 0)
                {
                    await _fileManager.DeleteAsync(fileEntity);
                }
                return new JsonResult(new ResponseModel(message: "Error while uploading file", code: HttpStatusCode.BadRequest, success: false,
                    errors: new List<string> { "Something went wrong" }));
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Download(long id)
        {

            var fileEntity = await _fileManager.GetAsync(id);
            if (fileEntity == null || !System.IO.File.Exists(Path.Combine(_env.WebRootPath, fileEntity.Path)))
            {
                return BadRequest();
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(_env.WebRootPath, fileEntity.Path));

            return File(fileBytes, "application/octet-stream", fileEntity.Name);
        }


    }
}