using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Web.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class PermissionAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string _permissionType ;

    public PermissionAuthorizeAttribute(string permissionType){
        _permissionType = permissionType;
      
    }
    
     public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {

        var roleAndPermissionService = context.HttpContext.RequestServices.GetRequiredService<IRoleService>();
        // skip authorization if action is decorated with [AllowAnonymous] attribute
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous)
            return;

        // authorization
        bool isLogin = context.HttpContext.User.Identity?.IsAuthenticated ?? false;
        if (!isLogin)
        {
            // not logged in or role not authorized
            context.Result = new ChallengeResult();
            return;
        }

        string? roleName = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        string? controller = context.RouteData.Values["controller"]?.ToString();

        if(string.IsNullOrEmpty(roleName) || string.IsNullOrEmpty(controller)){
            context.Result = new ForbidResult();
            return;
        }
        
        Response<bool> response = await roleAndPermissionService.HasPermission(roleName,controller,_permissionType);

        if(!response.Success || !response.Data){
            context.Result = new ForbidResult();
        }

        context.HttpContext.Items[Constants.CanEdit] = false;
        context.HttpContext.Items[Constants.CanView] = true;
        context.HttpContext.Items[Constants.CanDelete] = false;

        if(_permissionType.Equals(Constants.CanView)){
            Response<PagePermission?> permissionsResponse = await roleAndPermissionService.GetPermissionsForControllerAndRoleAsync(controller, roleName);
            if(permissionsResponse.Success){
                PagePermission permissions = permissionsResponse.Data!;
                context.HttpContext.Items[Constants.CanEdit] = permissions.Add;
                context.HttpContext.Items[Constants.CanDelete] = permissions.Delete;
            }
        }


    }
}
