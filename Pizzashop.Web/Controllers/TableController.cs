using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizzashop.Entity;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;
using Pizzashop.Web.Attributes;

namespace Pizzashop.Web.Controllers;

[Authorize]
[PermissionAuthorize(Constants.CanView)]
public class TableController : Controller
{

    private readonly ISectionService _sectionService;
    private readonly IOrderAppTableService _orderAppTableService;
    private readonly IWaitingTokensService _waitingTokensService;


    public TableController(ISectionService sectionService, IOrderAppTableService orderAppTableService, IWaitingTokensService waitingTokensService)
    {

        _sectionService = sectionService;
        _orderAppTableService = orderAppTableService;
        _waitingTokensService = waitingTokensService;
    }

    public async Task<IActionResult> Index()
    {
        var response = await _orderAppTableService.GetTablesForOrderApp();
        return View(response.Data);

    }
    

    public async Task<IActionResult> GetAllTokensBySection(int sectionId)
    {
        Response<IEnumerable<WaitingTokenList>> response = await _waitingTokensService.GetAllTokensBySection(sectionId);
        if (!response.Success)
        {
            return PartialView("_WaitingList");
        }

        IEnumerable<WaitingTokenList> model = response.Data!;
        return PartialView("_WaitingList", model);

    }

    public async Task<JsonResult> GetCustomer(int customerId)
    {
        Response<WaitingTokenList> response = await _waitingTokensService.GetCustomerAsync(customerId);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });

        }
        return Json(new { success = true, message = response.Message, data = response.Data });
    }
    public async Task<JsonResult> GetCustomerByEmails(string email)
    {
        Response<WaitingTokenList> response = await _waitingTokensService.GetCustomerByEmail(email);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });

        }
        return Json(new { success = true, message = response.Message, data = response.Data });
    }

    public async Task<IActionResult> GetTokenById(int id)
    {
        if (id == 0)
        {
            return PartialView("_TokenAddModal");
        }

        var response = await _waitingTokensService.GetTokenByIdAsync(id);
        WaitingTokenList model = response!;

        return PartialView("_TokenAddModal", model);
    }

    public async Task<IActionResult> CreateTokens(WaitingTokenList model)
    {

        if (!ModelState.IsValid)
        {
            return PartialView("_TokenAddModal");
        }

        var response = await _waitingTokensService.CreateToken(model);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }
        return Json(new { success = true, message = response.Message, data = response.Data });
    }

}
