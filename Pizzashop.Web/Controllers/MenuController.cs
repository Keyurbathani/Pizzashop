using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;
using Pizzashop.Web.Attributes;


namespace Pizzashop.Web.Controllers;

[Authorize]
[PermissionAuthorize(Constants.CanView)]
public class MenuController : Controller
{
    private readonly IMenuService _menuService;
    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public IActionResult Index()
    {
        string? canEdit = HttpContext.Items[Constants.CanEdit]?.ToString();
        string? canDelete = HttpContext.Items[Constants.CanDelete]?.ToString();
        HttpContext.Session.SetString(Constants.CanEdit, canEdit ?? string.Empty);
        HttpContext.Session.SetString(Constants.CanDelete, canDelete ?? string.Empty);
        return View();
    }

    public async Task<IActionResult> GetCategories()
    {
        Response<IEnumerable<CategoryVM>> response = await _menuService.GetAllCategoriesAsync();
        if (!response.Success)
        {
            return RedirectToAction("Index");
        }
        IEnumerable<CategoryVM> model = response.Data!;
        return PartialView("_Category", model);
    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<JsonResult> AddCategory(CategoryVM model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Incorrect Details!" });
        }

        Response<CategoryVM> response = await _menuService.AddCategoryAsync(model);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message, data = model });

    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<JsonResult> EditCategory(CategoryVM model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Incorrect Details!" });
        }

        Response<CategoryVM> response = await _menuService.EditCategoryAsync(model);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message, id = model.Id });
    }

    [PermissionAuthorize(Constants.CanDelete)]
    public async Task<JsonResult> DeleteCategory(int id)
    {
        if (id == 0)
        {
            return Json(new { success = false, message = "Id Is NULL" });
        }

        var response = await _menuService.DeleteCategoryAsync(id);

        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message, id = id });
    }


    public async Task<IActionResult> GetItemsByCategory(int id)
    {

        var response = await _menuService.GetItemsByCategoryId(id);
        if (response != null)
        {
            IEnumerable<ItemListVM> model = response!;
            return PartialView("_Items", model);
        }

        return RedirectToAction("Index");


    }

    public async Task<IActionResult> GetPagedItems(int categoryId, string searchString = "", int page = 1, int pagesize = 5, bool isASC = true)
    {
        if (categoryId == 0)
        {
            Response<IEnumerable<CategoryVM>> categoriesResponse = await _menuService.GetAllCategoriesAsync();
            if (!categoriesResponse.Success)
            {
                TempData["error"] = "No categories found!";
                return PartialView("_Items");
            }
            categoryId = categoriesResponse.Data!.FirstOrDefault()!.Id;
        }

        Response<PagedResult<ItemListVM>> response = await _menuService.GetPagedItemsAsync(categoryId, searchString, page, pagesize, isASC);
        if (!response.Success)
        {
            return PartialView("_Items");
        }

        PagedResult<ItemListVM> model = response.Data!;
        return PartialView("_Items", model);
    }

    public async Task<IActionResult> GetItemById(int id)
    {
        if (id == 0)
        {
            return PartialView("_AddItemModal");
        }

        var response = await _menuService.GetItemByIdAsync(id);
        ItemVM model = response!;

        return PartialView("_AddItemModal", model);
    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<IActionResult> AddItem(ItemVM model)
    {
        ModelState.Remove("Id");

        if (!ModelState.IsValid)
        {
            return PartialView("_AddItemModal", model);
        }

        var response = await _menuService.AddItemAsync(model);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });

        }

        return Json(new { success = true, message = response.Message, data = model });


    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<IActionResult> EditItem(ItemVM model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_AddItemModal", model);
        }

        var response = await _menuService.EditItemAsync(model);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message, data = model });


    }

    [PermissionAuthorize(Constants.CanDelete)]
    public async Task<JsonResult> DeleteItem(int id)
    {
        var response = await _menuService.DeleteItemAsync(id);
        if (response.Success)
        {
            return Json(new { success = true, message = response.Message });
        }
        return Json(new { success = false, message = response.Message });

    }

    // Return Dropdown List 
    public async Task<JsonResult> BindData()
    {
        SelectList? categories = null;
        SelectList? units = null;
        SelectList? modifier_groups = null;


        Response<IEnumerable<CategoryVM>> categoryResponse = await _menuService.GetAllCategoriesAsync();
        if (categoryResponse.Success)
        {
            categories = new(categoryResponse.Data, "Id", "Name");
        }

        Response<IEnumerable<UnitVM>> unitResponse = await _menuService.GetAllUnitsAsync();
        if (unitResponse.Success)
        {
            units = new(unitResponse.Data, "Id", "Name");
        }

        Response<IEnumerable<ModifierGroupVM>> modifierGroupResponse = await _menuService.GetAllModifierGroupsAsync();
        if (modifierGroupResponse.Success)
        {
            modifier_groups = new(modifierGroupResponse.Data, "Id", "Name");
        }


        return Json(new { categories, units, modifier_groups });
    }

    [PermissionAuthorize(Constants.CanDelete)]
    public async Task<JsonResult> DeleteMany(IEnumerable<int> ids)
    {
        if (!ids.Any())
        {
            return Json(new { success = false, message = "No Items Selected to Delete" });
        }



        var response = await _menuService.DeleteManyItemAsync(ids);
        if (response)
        {
            TempData["success"] = "Items deleted successfully";
            return Json(new { success = true, message = "Items deleted successfully" });
        }

        return Json(new { success = false });
    }


    // Modifiers

    public async Task<IActionResult> GetModifierGroups()
    {
        Response<IEnumerable<ModifierGroupVM>> response = await _menuService.GetAllModifierGroupsAsync();
        if (!response.Success)
        {
            return RedirectToAction("Index");
        }
        IEnumerable<ModifierGroupVM> model = response.Data!;
        return PartialView("_ModifierGroup", model);
    }

    public async Task<JsonResult> GetModifierGroupsForModifier(int id)
    {
        Response<IEnumerable<ModifierGroupVM>> response = await _menuService.GetModifierGroupsByModifierIdAsync(id);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }
        IEnumerable<ModifierGroupVM> data = response.Data!;
        return Json(new { success = true, data });
    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<IActionResult> AddModifierGroup(ModifierGroupVM model, List<int> modifierIds)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Incorrect Details!" });
        }


        Response<ModifierGroupVM?> response = await _menuService.AddModifierGroupAsync(model, modifierIds);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = "Modifier Group successfully added!", data = model });
    }


    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<IActionResult> EditModifierGroup(ModifierGroupVM model, List<int> modifierIds)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Incorrect Details!" });
        }

        Response<ModifierGroupVM?> response = await _menuService.EditModifierGroupAsync(model, modifierIds);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message, id = model.Id });
    }


    [PermissionAuthorize(Constants.CanDelete)]
    public async Task<JsonResult> DeleteModifierGroup(int id)
    {
        if (id == 0)
        {
            return Json(new { success = false, message = "Modifier Group not found!" });
        }


        Response<bool> response = await _menuService.DeleteModifierGroupAsync(id);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message });
    }

    public async Task<IActionResult> GetModifierById(int id)
    {
        if (id == 0)
        {
            return PartialView("_AddEditModifierModal");
        }

        Response<ModifierVM?> response = await _menuService.GetModifierByIdAsync(id);
        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return RedirectToAction("Index");
        }
        ModifierVM model = response.Data!;

        return PartialView("_AddEditModifierModal", model);
    }

    public async Task<IActionResult> GetModifiersByModifierGroup(int id)
    {
        if (id == 0)
        {
            Response<IEnumerable<ModifierGroupVM>> categoriesResponse = await _menuService.GetAllModifierGroupsAsync();
            if (!categoriesResponse.Success)
            {
                TempData["error"] = "No Modifier group found!";
                return PartialView("_Modifier");
            }
            id = categoriesResponse.Data!.FirstOrDefault()!.Id;
        }

        Response<IEnumerable<ModifierListVM>> response = await _menuService.GetModifiersByGroupIdAsync(id);
        if (!response.Success)
        {
            return RedirectToAction("Index");
        }
        IEnumerable<ModifierListVM> model = response.Data!;
        return PartialView("_Modifier", model);
    }

    public async Task<IActionResult> GetModifiersByModifierGroupPaged(int modifierGroupId, string searchString = "", int page = 1, int pagesize = 5, bool isASC = true)
    {
        if (modifierGroupId == 0)
        {
            Response<IEnumerable<ModifierGroupVM>> categoriesResponse = await _menuService.GetAllModifierGroupsAsync();
            if (!categoriesResponse.Success)
            {
                TempData["error"] = "No Modifier group found!";
                return PartialView("_Modifier");
            }
            modifierGroupId = categoriesResponse.Data!.FirstOrDefault()!.Id;
        }

        Response<PagedResult<ModifierListVM>> response = await _menuService.GetPagedMofiersAsync(modifierGroupId, searchString, page, pagesize, isASC);
        if (!response.Success)
        {
            return RedirectToAction("Index");
        }

        PagedResult<ModifierListVM> model = response.Data!;
        return PartialView("_Modifier", model);
    }



    public async Task<IActionResult> GetAllModifiers()
    {
        Response<IEnumerable<ModifierListVM>> response = await _menuService.GetAllModifiersAsync();
        if (!response.Success)
        {
            return RedirectToAction("Index");
        }
        IEnumerable<ModifierListVM> model = response.Data!;
        return PartialView("_SelectModifierModal", model);
    }
    public async Task<IActionResult> GetAllExistingModifiers(string searchStringFor = "", int page = 1, int pagesize = 5)
    {
        Response<PagedResult<ModifierListVM>> response = await _menuService.GetAllExistingModifiers(searchStringFor, page, pagesize);
        if (!response.Success)
        {
            return PartialView("_SelectModifierModal");
        }

        PagedResult<ModifierListVM> model = response.Data!;
        return PartialView("_SelectModifierModal", model);
    }

    public async Task<JsonResult> GetModifierDetailsByModifierGroup(int id)
    {
        Response<IEnumerable<ModifierListVM>> response = await _menuService.GetModifiersByGroupIdAsync(id);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }
        IEnumerable<ModifierListVM> data = response.Data!;
        return Json(new { success = true, data });
    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<JsonResult> AddModifier(ModifierVM model, List<int> modifierGroups)
    {
        ModelState.Remove("model.Id");
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Incorrect Details!" });
        }

        Response<ModifierVM?> response = await _menuService.AddModifierAsync(model, modifierGroups);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message });
    }

    [PermissionAuthorize(Constants.CanEdit)]

    public async Task<JsonResult> EditModifier(ModifierVM model, List<int> modifierGroups)
    {
        ModelState.Remove("model.Id");
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Incorrect Details!" });
        }

        Response<ModifierVM?> response = await _menuService.EditModfierAsync(model, modifierGroups);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message });
    }

    [PermissionAuthorize(Constants.CanDelete)]

    public async Task<JsonResult> DeleteModifier(int id)
    {
        if (id == 0)
        {
            return Json(new { success = false, message = "Modifier not found!" });
        }


        Response<bool> response = await _menuService.DeleteModifierAsync(id);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message });
    }

    [PermissionAuthorize(Constants.CanDelete)]
    public async Task<JsonResult> DeleteManyModifier(IEnumerable<int> ids)
    {
        if (!ids.Any())
        {
            return Json(new { success = false, message = "No Items Selected to Delete" });
        }

        Response<bool> response = await _menuService.DeleteManyModifierAsync(ids);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message });
    }


}
