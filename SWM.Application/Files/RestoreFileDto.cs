using System;
using System.Collections.Generic;
using System.Text;

namespace SWM.Application.Files
{
    public class RestoreFileDto : EntityDto<long>
    {
        public long ParentId { get; set; }
    }
}
