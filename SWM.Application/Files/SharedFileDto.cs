using SWM.Application.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWM.Application.Files
{
    public class SharedFileDto : EntityDto<long>
    {
        public string Name { get; set; }
        public DateTime? LastModificationTime { get; set; } = null;
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime? DeletionTime { get; set; } = null;
        public long ParentId { get; set; }
        public string Path { get; set; }
        public long UserId { get; set; }
        public UserDto Owner { get; set; }
        public double Size { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
