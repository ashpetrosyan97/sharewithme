using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Core.Users
{
    public interface IUserManager
    {
        Task CreateAsync(UserEntity input);
        Task UpdateAsync(UserEntity input);
        Task DeleteAsync(UserEntity input);
        Task<UserEntity> GetAsync(Expression<Func<UserEntity, bool>> predicate);
        Task<List<UserEntity>> GetAll();
    }
}
