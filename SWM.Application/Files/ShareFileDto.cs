using System;
using System.Collections.Generic;
using System.Text;

namespace SWM.Application.Files
{
    public class ShareFileDto
    {
        public long FileId { get; set; }
        public ICollection<long> UserId { get; set; }
    }
}
