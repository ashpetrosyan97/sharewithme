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
        private readonly IUnitOfWork uow;

        public AccountManager(IUnitOfWork uow)
        {
            this.uow = uow;
        }
        public async Task CreateAsync(AccountEntity input)
        {
            await uow.Repository<AccountEntity>().InsertAsync(input);
        }

        public async Task DeleteAsync(AccountEntity input)
        {
            await uow.Repository<AccountEntity>().DeleteAsync(input);
        }

        public async Task<List<AccountEntity>> GetAll()
        {
            return await uow.Repository<AccountEntity>().GetAll().ToListAsync();
        }

        public async Task<AccountEntity> GetAsync(long id)
        {
            return await uow.Repository<AccountEntity>().GetAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(AccountEntity input)
        {
            await uow.Repository<AccountEntity>().UpdateAsync(input);
        }
    }
}
