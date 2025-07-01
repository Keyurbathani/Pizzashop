using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Interfaces;

public interface IRoleService
{
    Task<List<Role>> GetRoles();
     Task<Response<IEnumerable<RoleVM>>> GetAllRolesAsync();

    Task<Role> GetRoleById(int id);

    Task<Response<bool>> HasPermission(string roleName, string controller, string permissionType);

    Task<Response<PagePermission?>> GetPermissionsForControllerAndRoleAsync(string controller, string roleName);


}

