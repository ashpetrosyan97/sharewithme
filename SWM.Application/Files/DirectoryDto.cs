using SWM.Application.Users;
using SWM.Core.Files;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWM.Application.Files
{
    public class DirectoryDto:EntityDto<long>
    {
        public string Name { get; set; }
        public FileEntityType Type { get; set; }
        public long ParentId { get; set; }
        public UserDto User { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
