using SWM.Core.Users;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWM.Core.Accounts
{
    [Table("Accounts")]
    public class AccountEntity : Entity<int>
    {
        public string Name { get; set; }
        public AccountTypes Type { get; set; }
        public decimal Price { get; set; }
        public int StorageSize { get; set; }
        public ICollection<UserEntity> Users { get; set; }
    }
}
