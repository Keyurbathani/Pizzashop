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
public class WaitingListController : Controller
{
    private readonly ISectionService _sectionService;
    private readonly IOrderAppTableService _orderAppTableService;
    private readonly IWaitingTokensService _waitingTokensService;


    public WaitingListController(ISectionService sectionService, IOrderAppTableService orderAppTableService, IWaitingTokensService waitingTokensService)
    {

        _sectionService = sectionService;
        _orderAppTableService = orderAppTableService;
        _waitingTokensService = waitingTokensService;
    }
    public async Task<IActionResult> Index()
    {
        var response = await _sectionService.GetAllSectionAsync();
        return View(response.Data);
    }

    public async Task<IActionResult> GetAllWaitingTokensBySection(int sectionId)
    {
        Response<IEnumerable<WaitingTokenList>> response = await _waitingTokensService.GetAllTokensBySection(sectionId);
        if (!response.Success)
        {
            return PartialView("_WaitingListNew");
        }

        IEnumerable<WaitingTokenList> model = response.Data!;
        return PartialView("_WaitingListNew", model);

    }

    public async Task<IActionResult> GetTokenById(int id)
    {
        if (id == 0)
        {
            return PartialView("_AddEditWaitingToken");
        }

        var response = await _waitingTokensService.GetTokenByIdAsync(id);
        WaitingTokenList model = response!;

        return PartialView("_AddEditWaitingToken", model);
    }

    public async Task<IActionResult> CreateToken(WaitingTokenList model)
    {
        ModelState.Remove("SectionName");
        if (!ModelState.IsValid)
        {
            return PartialView("_AddEditWaitingToken");
        }

        var response = await _waitingTokensService.CreateToken(model);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });

        }
        return Json(new { success = true, message = response.Message, data = response.Data });
    }
    public async Task<IActionResult> EditToken(WaitingTokenList model)
    {

        if (!ModelState.IsValid)
        {
            return PartialView("_AddEditWaitingToken" , model);
        }

        var response = await _waitingTokensService.EditToken(model);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });

        }
        return Json(new { success = true, message = response.Message, data = response.Data });
    }

    public async Task<JsonResult> DeleteToken(int id)
    {
        var response = await _waitingTokensService.DeleteToken(id);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }
        return Json(new { success = true, message = response.Message, data = response.Data });
    }

    [HttpPost]
    public async Task<JsonResult> AssignTable(int customerId, IEnumerable<int> ids, int tokenId)
    {
        var response = await _waitingTokensService.AssignTableAddOrder(customerId, ids, tokenId);

        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }
        return Json(new { success = true, message = response.Message, data = response.Data.Id });
    }


}
