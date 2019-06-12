using SWM.Application.Accounts;
using SWM.Application.Files;
using SWM.Core.Accounts;
using SWM.Core.Files;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWM.Application.Users
{
    public class UserDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Country { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public string PhoneNumber { get; set; }
        public AccountDto Account { get; set; }
        public long AccountId { get; set; }
        public double UsedSpace { get; set; }
        public double UsedSpacePercent { get; set; }
        public string ProfileImage { get; set; }
    }
}
