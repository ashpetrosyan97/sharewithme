using Microsoft.EntityFrameworkCore;
using SWM.Core.Accounts;
using SWM.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Core.Users
{
    public class UserManager : IUserManager
    {
        private readonly IRepository<UserEntity, long> _userRepository;

        public UserManager(IRepository<UserEntity, long> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task CreateAsync(UserEntity input)
        {
            await _userRepository.InsertAsync(input);
        }

        public async Task UpdateAsync(UserEntity input)
        {
            await _userRepository.UpdateAsync(input);
        }

        public async Task DeleteAsync(UserEntity input)
        {
            await _userRepository.DeleteAsync(input);
        }

        public async Task<UserEntity> GetAsync(Expression<Func<UserEntity, bool>> predicate)
        {
            return await _userRepository.GetAsync(predicate, x => x.Account, x => x.UsersSharedFiles);
        }

        public async Task<List<UserEntity>> GetAll()
        {
            return await _userRepository
                .GetAllIncluding(x => x.Account)
                .ToListAsync();
        }
    }
}
