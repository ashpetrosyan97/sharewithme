using SWM.Core.Files;
using SWM.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SWM.Core
{
    public class SharedFileEntity
    {
        public User User { get; set; }
        public long UserId { get; set; }
        public FileEntity File { get; set; }
        public long FileId { get; set; }
    }
}