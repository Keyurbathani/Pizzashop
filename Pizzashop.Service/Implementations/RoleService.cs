using Pizzashop.Entity.Constants;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Implementations;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;

    public RoleService(IRoleRepository roleRepository){

        _roleRepository = roleRepository;

    }
    public  async Task<List<Role>> GetRoles()
    {
        return await _roleRepository.GetAllRoles();      
    }

    public async Task<Response<IEnumerable<RoleVM>>> GetAllRolesAsync()
    {
        
            var roles = await _roleRepository.GetAllRoleFor();
            var rolelist = roles.Select(r => new RoleVM()
            {
                Id = r.Id,
                Name = r.Name
            });
            return Response<IEnumerable<RoleVM>>.SuccessResponse(rolelist,"Role Fetched Successfully.!");

    }

    public async Task<Role> GetRoleById(int id){
        return await _roleRepository.GetRoleById(id);
    }


    public async Task<Response<bool>> HasPermission(string roleName, string controller, string permissionType){
     
            IEnumerable<RolePermission> permissionsForRole = await _roleRepository.GetPermissionsForRoleByNameAsync(roleName);

            var controllerPermission = permissionsForRole.Where(rp => rp.Permission.ControllerName!.ToLower().Trim() == controller.ToLower().Trim()).ToList();
            bool result = false;
            if(controllerPermission.Any()){
                switch(permissionType){
                    case Constants.CanView:
                        result = controllerPermission.Any(cp => cp.CanView.GetValueOrDefault());
                        return Response<bool>.SuccessResponse(result, "permission fetched!");
                    case Constants.CanEdit:
                        result = controllerPermission.Any(cp => cp.CanAdd.GetValueOrDefault());
                        return Response<bool>.SuccessResponse(result, "permission fetched!");
                    case Constants.CanDelete:
                        result = controllerPermission.Any(cp => cp.CanDelete.GetValueOrDefault());
                        return Response<bool>.SuccessResponse(result, "permission fetched!");
                }
            }

            return Response<bool>.SuccessResponse(false, "permission fetched!");
        
    }

    public async Task<Response<PagePermission?>> GetPermissionsForControllerAndRoleAsync(string controller, string roleName)
    {
        
            RolePermission? controllerPermission = await _roleRepository.GetPermissionsForControllerAndRoleAsync(controller, roleName);
            if (controllerPermission == null)
            {
                return Response<PagePermission?>.FailureResponse("Permission not found!");
            }
            PagePermission permissions = new()
            {
                Id = controllerPermission.Permission.Id,
                Name = controllerPermission.Permission.ControllerName!,
                View = controllerPermission.CanView.GetValueOrDefault(),
                Add = controllerPermission.CanAdd.GetValueOrDefault(),
                Delete = controllerPermission.CanDelete.GetValueOrDefault()
            };

            return Response<PagePermission?>.SuccessResponse(permissions, "Permissions fetched successfylly!");
       
    }

   
}
