using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class RoleRepository : IRoleRepository
{
     private readonly PizzaShopContext _context;

    public RoleRepository(PizzaShopContext context)
    {
        _context = context;
    }

     public async Task<Role?> GetRoleById(int roleId)
    {
        return await _context.Roles.FirstOrDefaultAsync(c => c.Id == roleId);
    }

    public async Task<List<Role>> GetAllRoles()
    {
        return await _context.Roles.OrderBy(c => c.Name).ToListAsync();
    }
    
    public async Task<IEnumerable<Role>> GetAllRoleFor()
    {
        return await _context.Roles.OrderBy(c => c.Id).ToListAsync();
    }


    //for permissions
    public async Task<RolesPermissionVM> Permissions(int id)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
        if (role == null)
        {
           Console.WriteLine("role is null");
        }
        
        var permissions = await _context.RolePermissions.Where(p => p.RoleId == id && p.PermissionId < 8).Include(p => p.Permission).OrderBy(p=> p.PermissionId).Select(p => new PagePermission
        {
            Name = p.Permission.Name,
            Id = p.PermissionId,
            View = (bool)p.CanView!,
            Add = (bool)p.CanAdd!,
            Delete = (bool)p.CanDelete!

        }).ToListAsync();

        var RolesPermissionVM = new RolesPermissionVM
        {
            RoleId = role!.Id,
            RoleName = role.Name,
            PagePermission = permissions
        };
        return RolesPermissionVM;
    }

    public  void UpdatePermissions(RolesPermissionVM model)
    {
        var role = model.RoleId;
        var updatedPermissions = model.PagePermission;
        var rolesPermission = _context.RolePermissions;
        for (int i = 0; i < updatedPermissions!.Count; i++)
        {
            var pagePermissionAnd = rolesPermission.Where(p => p.RoleId == role && p.PermissionId == updatedPermissions[i].Id).FirstOrDefault();
            pagePermissionAnd!.CanView = updatedPermissions[i].View;
            pagePermissionAnd.CanAdd = updatedPermissions[i].Add;
            pagePermissionAnd.CanDelete = updatedPermissions[i].Delete;
            _context.Update(pagePermissionAnd);
        }
        _context.SaveChanges();
        
    }

    public async Task<IEnumerable<RolePermission>> GetPermissionsForRoleByNameAsync(string roleName)
    {
        return await _context.RolePermissions.Include(a => a.Role).Include(a => a.Permission).Where(a => a.Role.Name == roleName).ToListAsync();
    }

    public async Task<RolePermission?> GetPermissionsForControllerAndRoleAsync(string controller, string roleName)
    {
        return await _context.RolePermissions.Include(a => a.Role).Include(a => a.Permission).SingleOrDefaultAsync(a => a.Role.Name == roleName && a.Permission.ControllerName!.ToLower().Trim() == controller.ToLower().Trim());
    }
}

    

