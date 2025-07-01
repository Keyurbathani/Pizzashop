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
public class KOTController : Controller
{
    private readonly IMenuService _menuService;
    private readonly IOrderService _orderService;
    public KOTController(IMenuService menuService, IOrderService orderService)
    {
        _menuService = menuService;
        _orderService = orderService;
    }

    public async Task<IActionResult> Index()
    {
        Response<IEnumerable<CategoryVM>> response = await _menuService.GetAllCategoriesAsync();
        if (!response.Success)
        {
            return RedirectToAction("Index");
        }
        IEnumerable<CategoryVM> model = response.Data!;
        return View("Index", model); 
    }

    public async Task<IActionResult> GetOrderByCategory(int id, string status = "")
    {
        
        var response = await _orderService.GetOrderByCategory(id,status);

        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return PartialView("_KotOrders");
        }

        IEnumerable<KotOrderVM> model = response.Data!;
        return PartialView("_KotOrders" , model);

    }
   
    public async Task<JsonResult> UpdateOrderItemsStatus(int orderId  , string itemStatusNew , List<OrderItem> orderItems)
    {
        
        var response = await _orderService.UpdateOrderItemStatusAsync(orderId , itemStatusNew , orderItems);

        if (!response.Success)
        {
            return Json(new { success = false });
        }

         return Json(new { success = true, message = response.Message });

    }


}
