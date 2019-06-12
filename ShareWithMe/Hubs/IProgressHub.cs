using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareWithMe.Hubs
{
    public interface IProgressHub
    {
        Task SendUploadPercent(string uid, double percent);
        Task SendStorageInfo(double uid, double percent);
    }
}
