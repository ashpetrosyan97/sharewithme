using Microsoft.EntityFrameworkCore;
using SWM.Core.Accounts;
using SWM.Core.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SWM.Core.Users
{
    [Table("Users")]
    public class UserEntity : Entity<long>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Country { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public int AccountId { get; set; }
        [ForeignKey(nameof(AccountId))]
        [Required]
        public AccountEntity Account { get; set; }
        public DateTime? LastLoginTime { get; set; } = null;
        public DateTime CreationTime { get; set; } = DateTime.Now;


        public string FilesDirectory { get; set; }
        public string ProfileImage { get; set; }
        public virtual List<SharedFileEntity> UsersSharedFiles { get; set; }
    }
}
