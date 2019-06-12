using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SWM.Application.Files
{
    public class MoveFileDto:EntityDto<long>
    {
        [Required]
        public long ParentId { get; set; }
    }
}
