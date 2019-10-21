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
        public async Task CreateAsync(Account input)
        {
            await uow.Repository<Account>().InsertAsync(input);
        }

        public async Task DeleteAsync(Account input)
        {
            uow.Repository<Account>().Delete(input);
            await uow.CommitAsync();
        }

        public async Task<List<Account>> GetAll()
        {
            return await uow.Repository<Account>().GetAllEntities().ToListAsync();
        }

        public async Task<Account> GetAsync(long id)
        {
            return await uow.Repository<Account>().GetAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(Account input)
        {
            uow.Repository<Account>().Update(input);
            await uow.CommitAsync();
        }
    }
}
