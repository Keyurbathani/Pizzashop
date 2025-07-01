using System.Linq.Expressions;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;

namespace Pizzashop.Repository.Interfaces;

public interface IUserRepository
{
     Task<User?> GetUserByEmail(string email);

     Task<User?> GetUserByUserName(string username);

     Task<User?> GetUserByPhone(long phone);

     Task<User?> GetUserById(int id);

     Task<bool> IsExistsAsync(Expression<Func<User, bool>> predicate);

     Task UpdateUser(User user);

     Task Adduser(User user);
     Task<(IEnumerable<User> list, int totalRecords)> GetPagedUsersAsync(string searchString, int page, int pagesize, bool isASC, string sortColumn);


}
