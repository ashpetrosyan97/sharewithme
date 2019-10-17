using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Core.Users
{
    public interface IUserManager
    {
        Task CreateAsync(User input);
        Task UpdateAsync(User input);
        Task DeleteAsync(User input);
        Task<User> GetAsync(Expression<Func<User, bool>> predicate);
        Task<List<User>> GetAll();
    }
}
