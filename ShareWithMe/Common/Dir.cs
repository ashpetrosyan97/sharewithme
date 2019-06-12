using Microsoft.AspNetCore.Hosting;
using NReco.VideoInfo;
using SWM.Core.Files;
using SWM.Core.Users;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShareWithMe.Common
{
    public class Dir
    {
        public static async Task<double> GetUsedSpace(long id, IHostingEnvironment env, IFileManager fileManager)
        {
            var files = await fileManager.GetAll(id);

            long b = 0;
            foreach (var file in files.Where(x => x.Type != FileEntityType.Folder))
            {
                string path = Path.Combine(env.WebRootPath, file.Path);
                if (File.Exists(path))
                {
                    FileInfo info = new FileInfo(path);
                    b += info.Length;
                }
            }
            return Math.Round(b / Math.Pow(1024, 2), 1);
        }

        public static async Task<double> GetFreeSpase(UserEntity user, IHostingEnvironment env, IFileManager fileManager)
        {
            double usedSpace = await GetUsedSpace(user.Id, env, fileManager);
            return user.Account.StorageSize - usedSpace;
        }
        public static TimeSpan GetVideoDuration(string path)
        {
            var ffProbe = new FFProbe();
            return ffProbe.GetMediaInfo(path).Duration;
        }

    }
}
