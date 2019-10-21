using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Core.Accounts
{
    interface IAccountManager
    {
        Task CreateAsync(Account input);
        Task UpdateAsync(Account input);
        Task DeleteAsync(Account input);
        Task<Account> GetAsync(long id);
        Task<List<Account>> GetAll();
    }
}
