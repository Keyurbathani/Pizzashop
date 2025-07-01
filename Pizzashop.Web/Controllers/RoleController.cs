using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Interfaces;
using Pizzashop.Web.Attributes;

namespace Pizzashop.Web.Controllers;

[Authorize]
[PermissionAuthorize(Constants.CanView)]
public class RoleController : Controller
{

    private readonly IRoleService _roleService;
   
    private readonly IRoleRepository _roleRepository;

    public RoleController(IRoleService roleService, IRoleRepository roleRepository)
    {
        _roleService = roleService;
        _roleRepository = roleRepository;
        
    }

      public async Task<IActionResult> Roles()
    {
        string? canEdit = HttpContext.Items[Constants.CanEdit]?.ToString();
        string? canDelete = HttpContext.Items[Constants.CanDelete]?.ToString();
        HttpContext.Session.SetString(Constants.CanEdit, canEdit ?? string.Empty);
        HttpContext.Session.SetString(Constants.CanDelete, canDelete ?? string.Empty);

        var roleResponse = await _roleService.GetAllRolesAsync();
        return View(roleResponse.Data);
    }

    //for permissions
    public async Task<IActionResult> Permissions(int Id)
    {
        var model = await _roleRepository.Permissions(Id);
         if (model != null)
        {
            TempData["success"] = "Permissions Fetched Successfully";
        }

        return View(model);
    }

    [HttpPost]
    [PermissionAuthorize(Constants.CanEdit)]
    public  IActionResult UpdatePermissions(RolesPermissionVM model)
    {
        _roleRepository.UpdatePermissions(model);
         TempData["success"] = "Permissions Updated Successfully";
        return RedirectToAction("Roles");
    }
}
