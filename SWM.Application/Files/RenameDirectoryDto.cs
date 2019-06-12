using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SWM.Application.Files
{
    public class RenameDirectoryDto : EntityDto<long>
    {
        [Required]
        public string Name { get; set; }
    }
}
