using Microsoft.EntityFrameworkCore;
using SWM.Core.Accounts;
using SWM.Core.Repositories;
using SWM.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Core.Users
{
    public class UserManager : IUserManager
    {
        private readonly IUnitOfWork uow;
        public UserManager(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public async Task CreateAsync(User user)
        {
            try
            {
                var product = await uow.Repository<User>().InsertAsync(user);
                await uow.CommitAsync();
            }
            catch (Exception e)
            {
                uow.Rollback();
                if (e is DbUpdateException)
                    throw new CoreException(e.InnerException);
                else
                    throw new CoreException();
            }
        }

        public async Task UpdateAsync(User input)
        {
            uow.Repository<User>().Update(input);
            await uow.CommitAsync();
        }

        public async Task DeleteAsync(User input)
        {
            uow.Repository<User>().Delete(input);
        }

        public async Task<User> GetAsync(Expression<Func<User, bool>> predicate)
        {
            return await uow.Repository<User>().GetAsync(predicate, x => x.Include(u => u.Account).Include(x => x.UsersSharedFiles));
        }

        public async Task<List<User>> GetAll()
        {
            return await uow.Repository<User>()
                .GetAllEntities()
                .Include(x => x.Account)
                .ToListAsync();
        }
    }
}
