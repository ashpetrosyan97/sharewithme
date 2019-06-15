using MediaInfoLib;
using Microsoft.AspNetCore.Hosting;
using SWM.Core.Files;
using SWM.Core.Users;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        public static TimeSpan GetVideoDuration(string path, IHostingEnvironment env)
        {
            string ffMPEG = Path.Combine(env.ContentRootPath, "ffMPEG.exe");
            Process mProcess = null;

            StreamReader SROutput = null;
            string outPut = "";

            string param = string.Format("-i \"{0}\"", path);

            ProcessStartInfo oInfo = null;

            Regex re = null;
            Match m = null;
            TimeSpan Duration = TimeSpan.FromSeconds(5);

            //Get ready with ProcessStartInfo
            oInfo = new ProcessStartInfo(ffMPEG, param)
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false
            };

            // Lets start the process

            mProcess = Process.Start(oInfo);

            // Divert output
            SROutput = mProcess.StandardError;

            // Read all
            outPut = SROutput.ReadToEnd();

            // Please donot forget to call WaitForExit() after calling SROutput.ReadToEnd

            mProcess.WaitForExit();
            mProcess.Close();
            mProcess.Dispose();
            SROutput.Close();
            SROutput.Dispose();

            //get duration

            re = new Regex("[D|d]uration:.((\\d|:|\\.)*)");
            m = re.Match(outPut);

            if (m.Success)
            {
                //Means the output has cantained the string "Duration"
                string temp = m.Groups[1].Value;
                string[] timepieces = temp.Split(new char[] { ':', '.' });
                if (timepieces.Length == 4)
                {

                    // Store duration
                    Duration = new TimeSpan(0, Convert.ToInt16(timepieces[0]), Convert.ToInt16(timepieces[1]), Convert.ToInt16(timepieces[2]), Convert.ToInt16(timepieces[3]));
                }
            }
            return Duration;
        }


        static public TimeSpan GetVideoDuration(string fileName)
        {
            MediaInfo mi = new MediaInfo();
            mi.Open(fileName);
            return new VideoInfo(mi).Duration;
        }

    }

}
