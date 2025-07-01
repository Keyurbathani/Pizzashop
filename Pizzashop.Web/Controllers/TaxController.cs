using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;
using Pizzashop.Web.Attributes;

namespace Pizzashop.Web.Controllers;

[Authorize]
[PermissionAuthorize(Constants.CanView)]
public class TaxController : Controller
{

    private readonly ITaxService _taxService;

    public TaxController(ITaxService taxService)
    {

        _taxService = taxService;
    }
    public IActionResult Index()
    {

        string? canEdit = HttpContext.Items[Constants.CanEdit]?.ToString();
        string? canDelete = HttpContext.Items[Constants.CanDelete]?.ToString();
        HttpContext.Session.SetString(Constants.CanEdit, canEdit ?? string.Empty);
        HttpContext.Session.SetString(Constants.CanDelete, canDelete ?? string.Empty);

        return View();
    }

    public async Task<IActionResult> GetPagedItems(string searchString = "", int page = 1, int pagesize = 5, bool isASC = true)
    {

        Response<PagedResult<TaxListVM>> response = await _taxService.GetPagedItemsAsync(searchString, page, pagesize, isASC);
        if (!response.Success)
        {
            return PartialView("_TaxAndFee");
        }

        PagedResult<TaxListVM> model = response.Data!;
        return PartialView("_TaxAndFee", model);

    }

    public async Task<IActionResult> GetTaxById(int id)
    {
        if (id == 0)
        {
            return PartialView("_AddTaxModal");
        }

        var response = await _taxService.GetTaxByIdAsync(id);
        TaxVM model = response!;

        return PartialView("_AddTaxModal", model);
    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<JsonResult> AddTax(TaxVM model)
    {
        ModelState.Remove("Id");

        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Incorrect Details!" });
        }

        var response = await _taxService.AddTax(model, Request);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message, data = response.Data });


    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<JsonResult> EditTax(TaxVM model)
    {
       if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Incorrect Details!" });
        }

        var response = await _taxService.EditTax(model, Request);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message, data = response.Data });
    }

    [PermissionAuthorize(Constants.CanDelete)]
    public async Task<IActionResult> DeleteTax(int id)
    {
        await _taxService.DeleteTax(id, Request);
        return RedirectToAction("Index");
    }
}
