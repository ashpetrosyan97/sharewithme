using SWM.Core.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWM.Application.Accounts
{
    public class AccountDto: EntityDto<int>
    {
        public string Name { get; set; }
        public AccountTypes Type { get; set; }
        public decimal Price { get; set; }
        public int StorageSize { get; set; }
    }
}
