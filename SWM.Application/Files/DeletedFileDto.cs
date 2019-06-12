using SWM.Core.Files;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWM.Application.Files
{
    public class DeletedFileDto : EntityDto<long>
    {
        public string Name { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? DeletionTime { get; set; }
    }
}
