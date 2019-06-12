using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareWithMe.Common
{
    public static class AppConsts
    {
        public static string StorageDirectory { get; set; } = "StorageDirectory";
        public static string ProfileImagesDirectory { get; set; } = "ProfileImagesDirectory";
        public static IReadOnlyList<string> Extentions { get; set; } =
            new List<string>() { ".mp4", ".mpg", ".m4v", ".flv", ".avi", ".mov", ".wmv", ".webm", ".f4v", ".mpeg", ".vob" };
    }
}
