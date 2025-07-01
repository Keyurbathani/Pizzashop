using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;
using Pizzashop.Web.Attributes;

namespace Pizzashop.Web.Controllers;

[Authorize]
[PermissionAuthorize(Constants.CanView)]
public class SectionController : Controller
{
    private readonly ISectionService _sectionService;
    public SectionController(ISectionService sectionService)
    {
        _sectionService = sectionService;
    }
    public IActionResult Index()
    {

        string? canEdit = HttpContext.Items[Constants.CanEdit]?.ToString();
        string? canDelete = HttpContext.Items[Constants.CanDelete]?.ToString();
        HttpContext.Session.SetString(Constants.CanEdit, canEdit ?? string.Empty);
        HttpContext.Session.SetString(Constants.CanDelete, canDelete ?? string.Empty);

        return View();
    }

    public async Task<IActionResult> GetSections()
    {
        Response<IEnumerable<SectionVM>> response = await _sectionService.GetAllSectionAsync();
        if (!response.Success)
        {
            return RedirectToAction("Index");
        }
        IEnumerable<SectionVM> model = response.Data!;
        return PartialView("_Section", model);
    }

    public async Task<IActionResult> GetPagedTables(int SectionId, string searchString = "", int page = 1, int pagesize = 5, bool isASC = true)
    {
        if (SectionId == 0)
        {
            Response<IEnumerable<SectionVM>> sectionResponse = await _sectionService.GetAllSectionAsync();
            if (!sectionResponse.Success)
            {
                TempData["error"] = "No sections found!";
                return PartialView("_Tables");
            }

            SectionId = sectionResponse.Data!.FirstOrDefault()!.Id;
        }

        Response<PagedResult<TableListVM>> response = await _sectionService.GetPagedItemsAsync(SectionId, searchString, page, pagesize, isASC);
        if (!response.Success)
        {
            return PartialView("_Tables");
        }

        PagedResult<TableListVM> model = response.Data!;
        return PartialView("_Tables", model);

    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<JsonResult> AddSection(SectionVM model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Incorrect Details!" });
        }

        Response<SectionVM?> response = await _sectionService.AddSection(model, Request);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message, data = model });
    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<JsonResult> EditSection(SectionVM model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Incorrect Details!" });
        }

        Response<SectionVM?> response = await _sectionService.EditSection(model, Request);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message, id = model.Id });
    }

    [PermissionAuthorize(Constants.CanDelete)]
    public async Task<JsonResult> DeleteSection(int id)
    {
        if (id == 0)
        {
            return Json(new { success = false, message = "Id Is NULL" });
        }

        var response = await _sectionService.DeleteSection(id, Request);

        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message, id = id });

    }

    public async Task<IActionResult> GetTableById(int id)
    {
        if (id == 0)
        {
            return PartialView("_AddTablesModal");
        }

        var response = await _sectionService.GetTableByIdAsync(id);
        TableVM model = response!;

        return PartialView("_AddTablesModal", model);
    }

    public async Task<JsonResult> BindData()
    {
        SelectList? sections = null;


        Response<IEnumerable<SectionVM>> categoryResponse = await _sectionService.GetAllSectionAsync();
        if (categoryResponse.Success)
        {
            sections = new(categoryResponse.Data, "Id", "Name");
        }

        return Json(new { sections });
    }
    public async Task<JsonResult> BindDataForSection()
    {
        SelectList? sections = null;

        Response<IEnumerable<SectionVM>> categoryResponse = await _sectionService.GetAllSectionAsync();
        if (categoryResponse.Success)
        {
            sections = new(categoryResponse.Data, "Id", "Name");
        }
        return Json(new { sections });
    }

    public async Task<JsonResult> BindDataForTable(int sectionId)
    { 
        
        Response<IEnumerable<TableVM>> tableResponse = await _sectionService.GetAllTableVM(sectionId);
        return Json(new { tableResponse.Data});
    }


    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<IActionResult> AddTables(TableVM model)
    {
        ModelState.Remove("Id");

        if (!ModelState.IsValid)
        {
            return PartialView("_AddTablesModal" , model);
        }

        var response = await _sectionService.AddTables(model, Request);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }
        return Json(new { success = true, message = response.Message , data = model });

    }


    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<IActionResult> EditTables(TableVM model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_AddTablesModal" , model);
        }

        var response = await _sectionService.EditTables(model, Request);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }
        
        return Json(new { success = true, message = response.Message , data = model });

    }

    [PermissionAuthorize(Constants.CanDelete)]
    public async Task<JsonResult> DeleteTable(int id)
    {
        var response = await _sectionService.DeleteTable(id, Request);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }
        return Json(new { success = true , message = response.Message});
    }


    [PermissionAuthorize(Constants.CanDelete)]
    public async Task<JsonResult> DeleteMany(IEnumerable<int> ids)
    {
        if (!ids.Any())
        {
            return Json(new { success = false, message = "No Items Selected to Delete" });
        }

        var response = await _sectionService.DeleteManyTableAsync(ids, Request);
        if (response.Success)
        {
            return Json(new { success = true, message = response.Message });
        }
        return Json(new { success = false , message = response.Message});
    }
}
