using Microsoft.EntityFrameworkCore;
using SWM.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Core.Accounts
{
    public class AccountManager : IAccountManager
    {
        public readonly IRepository<AccountEntity, int> _accountReposytory;

        public AccountManager(IRepository<AccountEntity, int> accountReposytory)
        {
            _accountReposytory = accountReposytory;
        }
        public async Task CreateAsync(AccountEntity input)
        {
            await _accountReposytory.InsertAsync(input);
        }

        public async Task DeleteAsync(AccountEntity input)
        {
            await _accountReposytory.DeleteAsync(input);
        }

        public async Task<List<AccountEntity>> GetAll()
        {
            return await _accountReposytory.GetAll().ToListAsync();
        }

        public async Task<AccountEntity> GetAsync(long id)
        {
            return await _accountReposytory.GetAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(AccountEntity input)
        {
            await _accountReposytory.UpdateAsync(input);
        }
    }
}
