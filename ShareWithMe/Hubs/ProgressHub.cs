using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using SWM.Core.Files;
using SWM.Core.Users;
using ShareWithMe.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareWithMe.Hubs
{
    [Authorize]
    public class ProgressHub : Hub<IProgressHub>
    {
        public static ConcurrentDictionary<long, string> Connections = new ConcurrentDictionary<long, string>();
        public readonly IUserManager _userManager;
        public readonly IFileManager _fileManager;
        private readonly IHostingEnvironment _env;
        public ProgressHub(IUserManager usermanager, IHostingEnvironment env, IFileManager fileManager)
        {
            _env = env;
            _userManager = usermanager;
            _fileManager = fileManager;
        }
        public override async Task OnConnectedAsync()
        {

            var userId = long.Parse(Context.User.Identity.Name);

            Connections.AddOrUpdate(userId, Context.ConnectionId, (key, oldValue) => Context.ConnectionId);
            await base.OnConnectedAsync();
            var user = await _userManager.GetAsync(u => u.Id == userId);
            double usedSpace = await Dir.GetUsedSpace(userId, _env, _fileManager) / 1024;
            double percent = 100 * usedSpace / (user.Account.StorageSize / 1024);
            await SendStorageInfo(usedSpace, percent);
        }

        public Task SendUploadPercent(string uid, double percent)
        {
            return Clients.Caller.SendUploadPercent(uid, percent);
        }
        public Task SendStorageInfo(double size, double percent)
        {
            return Clients.Caller.SendStorageInfo(size, percent);
        }

    }
}
