using SWM.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWM.Application.Files
{
    public class FileDto : EntityDto<long>
    {
        public string Name { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? DeletionTime { get; set; }
        public long ParentId { get; set; }
        public string Path { get; set; }
        public ICollection<SharedFileEntity> UsersSharedFiles { get; set; }
        public double Size { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
