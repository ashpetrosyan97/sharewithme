using SWM.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SWM.Core.Files
{
    [Table("Files")]
    public class FileEntity : Entity<long>
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public FileEntityType Type { get; set; }

        public DateTime? LastModificationTime { get; set; } = null;
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime? DeletionTime { get; set; } = null;
        public long ParentId { get; set; }
        public string Path { get; set; }
        public long OwnerId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public virtual List<SharedFileEntity> UsersSharedFiles { get; set; }
    }
}
