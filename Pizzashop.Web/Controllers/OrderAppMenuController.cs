using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;
using Pizzashop.Web.Attributes;

namespace Pizzashop.Web.Controllers;

[Authorize]
[PermissionAuthorize(Constants.CanView)]
public class OrderAppMenuController : Controller
{
    private readonly IUserService _userService;
    private readonly IOrderAppMenuService _orderAppService;
    private readonly IMenuService _menuService;
    private readonly IOrderService _orderService;

    public OrderAppMenuController(IUserService userService, IOrderAppMenuService orderAppService, IMenuService menuService, IOrderService orderService)
    {
        _userService = userService;
        _orderAppService = orderAppService;
        _menuService = menuService;
        _orderService = orderService;
    }

    #region Menu

    public async Task<IActionResult> GetAllCategories()
    {
        Response<IEnumerable<CategoryVM>> response = await _menuService.GetAllCategoriesAsync();
        return Json(response);
    }

    public async Task<IActionResult> Menu(int id)
    {
        Response<OrderDetailsVM> response = await _orderService.GetOrderByIdAsync(id);

        OrderDetailsVM? model = response.Data;
        if (model == null || (model.OrderStatus != "Pending" && model.OrderStatus != "InProgress" &&  model.OrderStatus != "Served" && model.OrderStatus != "Completed"))
        {
            return View();
        }
        return View(response.Data);
    }

    public async Task<IActionResult> GetItemsByCategoryId(int categoryId)
    {
        Response<IEnumerable<ItemListVM>> response = await _menuService.GetAvailableItemsByCategoryIdAsync(categoryId);
        return PartialView("_MenuItems", response.Data);
    }

    public async Task<IActionResult> GetFavoriteItems()
    {
        Response<IEnumerable<ItemListVM>> response = await _menuService.GetFavoriteItemsAsync();
        return PartialView("_MenuItems", response.Data);
    }

    public async Task<JsonResult> ToggleFavoriteItem(int itemId)
    {
        Response<bool> response = await _menuService.ToggleFavoriteItemAsync(itemId);
        return Json(response);
    }

    public async Task<IActionResult> GetModifierGroupsForItem(int itemId)
    {
        Response<IEnumerable<ModifierGroupForItemVM>> response = await _menuService.GetApplicableModifiersForItem(itemId);
        return PartialView("_AddModifiersModal", response.Data);
    }

    public async Task<JsonResult> EditCustomerDetails(CustomerDetails model)
    {
        if (model.Capacity < model.NoOfPerson)
        {
            return Json(new { success = false, message = "Exceed Table Capicity!" });
        }

        Response<CustomerDetails?> response = await _orderAppService.UpdateCustomerDetailsAsync(model);
        return Json(response);
    }

    public async Task<JsonResult> EditOrderComment(int id, string? comment)
    {


        Response<string?> response = await _orderAppService.EditOrderCommentAsync(id, comment);

        return Json(response);
    }

    public IActionResult GetCustomerDetailsPartial(CustomerDetails model)
    {
        return PartialView("_MenuCustomerDetails", model);
    }

    public async Task<JsonResult> SaveOrder(OrderDetailsVM model)
    {
        Response<bool> response = await _orderAppService.SaveOrderAsync(model);
        return Json(response);

    }

    public async Task<JsonResult> IsItemQuantityPrepared(int orderItemId, int quantity)
    {
        Response<bool> response = await _orderAppService.IsItemQuantityPrepared(orderItemId, quantity);
        return Json(response);
    }

    public async Task<JsonResult> IsOrderServed(int orderId)
    {
        Response<bool> response = await _orderAppService.IsOrderServedAsync(orderId);
        return Json(response);
    }

    public async Task<JsonResult> CompleteOrder(int orderId, string paymentMethod)
    {
        Response<bool> response = await _orderAppService.CompleteOrderAsync(orderId, paymentMethod);
        return Json(response);
    }
    public async Task<JsonResult> ReviewOrder(int orderId, decimal foodCount, decimal serviceCount, decimal ambienceCount , string reviewComment)
    {
        Response<bool> response = await _orderAppService.ReviewOrder(orderId, foodCount, serviceCount, ambienceCount , reviewComment);
        return Json(response);
    }
    public async Task<JsonResult> CancelOrder(int orderId)
    {
        Response<bool> response = await _orderAppService.CancelOrderAsync(orderId);
        return Json(response);
    }
    public async Task<JsonResult> SaveItemComment(int id, string ItemComment)
    {
        Response<bool> response = await _orderAppService.SaveItemComment(id , ItemComment);
        return Json(response);
    }
    public async Task<JsonResult> GetItemComment(int id)
    {
        Response<string> response = await _orderAppService.GetItemComment(id);
        return Json(response.Data);
    }



    #endregion Menu

}
