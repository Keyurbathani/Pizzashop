using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Interfaces;

public interface IUserService
{
    Task<Response<PagedResult<UserNewListVM>>> GetPagedUsersAsync(string searchString, int page, int pagesize, bool isASC, string sortColumn);

    Task<Response<User>> CreateUser(AddNewUserModel model);

    Task<Response<User>> EditUserView(AddNewUserModel model);

   Task<Response<User>> EditUserById(AddNewUserModel model);

   Task<Response<bool>> DeleteUser(int id);
   
}
