using Pizzashop.Entity.ViewModels;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Interfaces;

public interface IOrderAppMenuService
{
    Task<Response<string?>> EditOrderCommentAsync(int id, string? comment);

    Task<Response<bool>> SaveOrderAsync(OrderDetailsVM model);

    Task<Response<bool>> IsItemQuantityPrepared(int orderItemId, int quantity);

    Task<Response<bool>> CompleteOrderAsync(int orderId, string paymentMethod);

    Task<Response<bool>> IsOrderServedAsync(int orderId);
    Task<Response<CustomerDetails?>> UpdateCustomerDetailsAsync(CustomerDetails model);
    Task<Response<bool>> ReviewOrder(int orderId , decimal foodCount , decimal serviceCount, decimal ambienceCount , string reviewComment);
    Task<Response<bool>> CancelOrderAsync(int orderId);
    Task<Response<bool>> SaveItemComment(int id, string ItemComment);
    Task<Response<string>> GetItemComment(int id);
    
}
