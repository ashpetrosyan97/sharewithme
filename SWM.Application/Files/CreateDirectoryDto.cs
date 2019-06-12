using SWM.Core.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SWM.Application.Files
{
    public class CreateDirectoryDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public FileEntityType Type { get; set; }
        public long ParentId { get; set; }
    }
}
