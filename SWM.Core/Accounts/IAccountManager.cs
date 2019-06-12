using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Core.Accounts
{
    interface IAccountManager
    {
        Task CreateAsync(AccountEntity input);
        Task UpdateAsync(AccountEntity input);
        Task DeleteAsync(AccountEntity input);
        Task<AccountEntity> GetAsync(long id);
        Task<List<AccountEntity>> GetAll();
    }
}
