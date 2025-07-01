using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;

namespace Pizzashop.Repository.Interfaces;

public interface IRoleRepository
{
    Task<Role> GetRoleById(int roleId);
    Task<List<Role>> GetAllRoles();

    Task<IEnumerable<Role>> GetAllRoleFor();

    Task<RolesPermissionVM> Permissions(int id);

    void UpdatePermissions(RolesPermissionVM model);

    Task<IEnumerable<RolePermission>> GetPermissionsForRoleByNameAsync(string roleName);

    Task<RolePermission?> GetPermissionsForControllerAndRoleAsync(string controller, string roleName);

}

